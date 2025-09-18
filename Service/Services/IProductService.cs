using ApplicationTest.Dtos;

public interface IProductService
{
    Task<(IEnumerable<ProductView> data, int total)> GetListAsync(ProductListRequest req, CancellationToken ct);
    Task<ProductView?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<Guid> CreateAsync(ProductCreateDto dto, Guid currentUserId, CancellationToken ct);
    Task UpdateAsync(ProductUpdateDto dto, CancellationToken ct);
    Task DeleteAsync(Guid id, CancellationToken ct);
}