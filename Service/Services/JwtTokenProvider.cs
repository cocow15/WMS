// Services/JwtTokenProvider.cs
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ApplicationTest.Entities;
using ApplicationTest.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ApplicationTest.Services;

public class JwtTokenProvider : IJwtTokenProvider
{
    private readonly IConfiguration _cfg;
    public JwtTokenProvider(IConfiguration cfg) => _cfg = cfg;

    public string Generate(User user)
    {
        var jwt = _cfg.GetSection("Jwt");

        // ambil nilai dengan default & validasi
        var issuer  = jwt["Issuer"]  ?? throw new InvalidOperationException("Missing Jwt:Issuer");
        var audience= jwt["Audience"]?? throw new InvalidOperationException("Missing Jwt:Audience");
        var keyStr  = jwt["Key"];
        if (string.IsNullOrWhiteSpace(keyStr) || keyStr.Length < 32)
            throw new InvalidOperationException("Missing/short Jwt:Key (min 32 chars).");

        var expiresMinutes = jwt.GetValue<int?>("ExpiresMinutes") ?? 120;

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var key   = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyStr));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiresMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}