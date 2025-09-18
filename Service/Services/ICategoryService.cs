using ApplicationTest.Dtos;

public interface ICategoryService
{
    Task<Guid> CreateAsync(CategoryCreateDto dto, CancellationToken ct);
    Task UpdateAsync(CategoryUpdateDto dto, CancellationToken ct);
    Task DeleteAsync(Guid id, CancellationToken ct);
    Task<IEnumerable<CategoryView>> GetAllAsync(CancellationToken ct);
}