using ApplicationTest.Data;
using ApplicationTest.Dtos;
using ApplicationTest.Services;
using Microsoft.EntityFrameworkCore;

namespace ApplicationTest.Services;

public class HostProductService : IHostProductService
{
    private readonly IHttpClientFactory _http;
    private readonly AppDbContext _db;

    public HostProductService(IHttpClientFactory http, AppDbContext db)
    {
        _http = http; _db = db;
    }

    private async Task<string?> GetTokenAsync(CancellationToken ct)
        => await _db.ExternalAuthTokens.OrderByDescending(x => x.IssuedAt).Select(x => x.Token).FirstOrDefaultAsync(ct);

    private static (int code, string body, string contentType) ToTuple(HttpResponseMessage res, string body)
    {
        var code = res.IsSuccessStatusCode ? 200 : (int)res.StatusCode; // <-- cast
        var ct   = res.Content.Headers.ContentType?.ToString() ?? "application/json";
        return (code, body, ct);
    }


    public async Task<(int code, string content, string contentType)> CreateAsync(ProductCreateRequest req, CancellationToken ct)
    {
        var token = await GetTokenAsync(ct);
        if (token is null) return (400, "{\"error\":\"No external token saved. Call /external/login first.\"}", "application/json");

        var client = _http.CreateClient("externalHost");
        client.DefaultRequestHeaders.Remove("token");
        client.DefaultRequestHeaders.Add("token", token);

        var payload = new {
            sku = req.sku, name = req.name, description = req.description,
            brandId = req.brandId, brand = req.brand,
            categoryId = req.categoryId, category = req.category,
            status = req.status, createdBy = req.createdBy
        };

        var res = await client.PostAsJsonAsync("/api/service.asmx/create", payload, ct);
        var body = await res.Content.ReadAsStringAsync(ct);
        var (code, text, ctType) = ToTuple(res, body);
        return (code, text, ctType);
    }

    public async Task<(int code, string content, string contentType)> UpdateAsync(ProductUpdateRequest req, CancellationToken ct)
    {
        var token = await GetTokenAsync(ct);
        if (token is null) return (400, "{\"error\":\"No external token saved.\"}", "application/json");

        var client = _http.CreateClient("externalHost");
        client.DefaultRequestHeaders.Remove("token");
        client.DefaultRequestHeaders.Add("token", token);

        var payload = new {
            productId = req.id.ToString().ToUpper(),
            sku = req.sku, name = req.name, description = req.description,
            brandId = req.brandId, brand = req.brand,
            categoryId = req.categoryId, category = req.category,
            status = req.status
        };

        var res = await client.PostAsJsonAsync("/api/service.asmx/update", payload, ct);
        var body = await res.Content.ReadAsStringAsync(ct);
        var (code, text, ctType) = ToTuple(res, body);
        return (code, text, ctType);
    }

    public async Task<(int code, string content, string contentType)> DeleteAsync(GetHostProductReq req, CancellationToken ct)
    {
        var token = await GetTokenAsync(ct);
        if (token is null) return (400, "{\"error\":\"No external token saved.\"}", "application/json");

        var client = _http.CreateClient("externalHost");
        client.DefaultRequestHeaders.Remove("token");
        client.DefaultRequestHeaders.Add("token", token);

        var payload = new { productId = req.productId.ToString().ToUpper() };
        var res = await client.PostAsJsonAsync("/api/service.asmx/delete", payload, ct);
        var body = await res.Content.ReadAsStringAsync(ct);
        var (code, text, ctType) = ToTuple(res, body);
        return (code, text, ctType);
    }

    public async Task<(int code, string content, string contentType)> GetByIdAsync(GetHostProductReq req, CancellationToken ct)
    {
        var token = await GetTokenAsync(ct);
        if (token is null) return (400, "{\"error\":\"No external token saved.\"}", "application/json");

        var client = _http.CreateClient("externalHost");
        client.DefaultRequestHeaders.Remove("token");
        client.DefaultRequestHeaders.Add("token", token);

        var payload = new { productId = req.productId.ToString().ToUpper() };
        var res = await client.PostAsJsonAsync("/api/service.asmx/getproductbyid", payload, ct);
        var body = await res.Content.ReadAsStringAsync(ct);
        var (code, text, ctType) = ToTuple(res, body);
        return (code, text, ctType);
    }

    public async Task<(int code, string content, string contentType)> ListAsync(CancellationToken ct)
    {
        var token = await GetTokenAsync(ct);
        if (token is null) return (400, "{\"error\":\"No external token saved.\"}", "application/json");

        var client = _http.CreateClient("externalHost");
        client.DefaultRequestHeaders.Remove("token");
        client.DefaultRequestHeaders.Add("token", token);

        var res = await client.GetAsync("/api/service.asmx/list", ct);
        var body = await res.Content.ReadAsStringAsync(ct);
        var (code, text, ctType) = ToTuple(res, body);
        return (code, text, ctType);
    }
}
