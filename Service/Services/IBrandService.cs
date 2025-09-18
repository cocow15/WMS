using ApplicationTest.Dtos;

public interface IBrandService
{
    Task<Guid> CreateAsync(BrandCreateDto dto, CancellationToken ct);
    Task UpdateAsync(BrandUpdateDto dto, CancellationToken ct);
    Task DeleteAsync(Guid id, CancellationToken ct);
    Task<IEnumerable<BrandView>> GetAllAsync(CancellationToken ct);
}