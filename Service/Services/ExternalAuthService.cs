using System.Globalization;
using System.Text.Json;
using ApplicationTest.Data;
using ApplicationTest.Dtos;
using ApplicationTest.Entities;
using ApplicationTest.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace ApplicationTest.Services;

public class ExternalAuthService : IExternalAuthService
{
    private readonly IHttpClientFactory _http;
    private readonly AppDbContext _db;
    private readonly IHttpContextAccessor _httpCtx;

    public ExternalAuthService(IHttpClientFactory http, AppDbContext db, IHttpContextAccessor httpCtx)
    {
        _http = http; _db = db; _httpCtx = httpCtx;
    }

    private Guid? GetCurrentUserId()
    {
        var user = _httpCtx.HttpContext?.User;
        if (user is null) return null;

        var uidStr =
            user.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
            user.FindFirstValue(ClaimTypes.NameIdentifier);

        return Guid.TryParse(uidStr, out var g) ? g : null;
    }

    // helper: ambil JSON object pertama (untuk respon ASMX yang suka ada noise)
    static string FirstJsonObject(string s)
    {
        int depth = 0; bool inString = false, escape = false;
        for (int i = 0; i < s.Length; i++)
        {
            char c = s[i];
            if (escape) { escape = false; continue; }
            if (c == '\\') { if (inString) escape = true; continue; }
            if (c == '"') { inString = !inString; continue; }
            if (!inString)
            {
                if (c == '{') depth++;
                else if (c == '}')
                {
                    depth--;
                    if (depth == 0) return s[..(i + 1)];
                }
            }
        }
        return s;
    }

     public async Task<(bool saved, DateTimeOffset? expiresAt)> LoginAndSaveAsync(ExternalLoginRequest req, CancellationToken ct)
    {
        var client = _http.CreateClient("externalHost");
        var res = await client.PostAsJsonAsync("/api/service.asmx/login",
            new { username = req.username, password = req.password }, ct);

        if (!res.IsSuccessStatusCode) return (false, null);

        var raw  = await res.Content.ReadAsStringAsync(ct);
        var main = FirstJsonObject(raw);

        using var doc = JsonDocument.Parse(main);
        if (!doc.RootElement.TryGetProperty("response", out var resp) ||
            !resp.TryGetProperty("data", out var data) ||
            !data.TryGetProperty("token", out var tokenEl) ||
            tokenEl.ValueKind != JsonValueKind.String)
        {
            return (false, null);
        }

        var token = tokenEl.GetString();
        DateTimeOffset? expiresAt = null;
        if (data.TryGetProperty("token_expired", out var expEl) && expEl.ValueKind == JsonValueKind.String
            && DateTime.TryParse(expEl.GetString(), out var expDt))
        {
            expiresAt = new DateTimeOffset(DateTime.SpecifyKind(expDt, DateTimeKind.Utc));
        }

        var currentUserId = GetCurrentUserId();
        if (currentUserId is null)
            throw new UnauthorizedAccessException("Missing user id claim.");

        await using var tx = await _db.Database.BeginTransactionAsync(ct);
        try
        {
            _db.ExternalAuthTokens.Add(new ExternalAuthToken
            {
                Id         = Guid.NewGuid(),       // sesuaikan nama properti entity-mu
                Username   = req.username,
                Token      = token!,
                IssuedAt   = DateTimeOffset.UtcNow,
                ExpiresAt  = expiresAt,
                RawResponse= raw,
                UserId     = currentUserId.Value   // <— PENTING: isi FK
            });

            await _db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
            return (true, expiresAt);
        }
        catch
        {
            await tx.RollbackAsync(ct);
            throw;
        }
    }

    public async Task<string?> GetLatestTokenAsync(CancellationToken ct)
        => await _db.ExternalAuthTokens
            .OrderByDescending(x => x.IssuedAt)
            .Select(x => x.Token)
            .FirstOrDefaultAsync(ct);
}
