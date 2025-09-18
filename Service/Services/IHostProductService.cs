using ApplicationTest.Dtos;

namespace ApplicationTest.Services;

public interface IHostProductService
{
    Task<(int code, string content, string contentType)> CreateAsync(ProductCreateRequest req, CancellationToken ct);
    Task<(int code, string content, string contentType)> UpdateAsync(ProductUpdateRequest req, CancellationToken ct);
    Task<(int code, string content, string contentType)> DeleteAsync(GetHostProductReq req, CancellationToken ct);
    Task<(int code, string content, string contentType)> GetByIdAsync(GetHostProductReq req, CancellationToken ct);
    Task<(int code, string content, string contentType)> ListAsync(CancellationToken ct);
}
