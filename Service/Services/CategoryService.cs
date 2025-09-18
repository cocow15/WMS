using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ApplicationTest.Dtos;
using ApplicationTest.Entities;
using ApplicationTest.Repositories;

namespace ApplicationTest.Services;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public CategoryService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<Guid> CreateAsync(CategoryCreateDto dto, CancellationToken ct)
    {
        var repo = _uow.Repo<Category>();

        var existed = await repo.Query()
            .FirstOrDefaultAsync(x => x.Name.ToLower() == dto.Name.ToLower(), ct);

        if (existed != null) return existed.CategoryId;

        var entity = new Category
        {
            CategoryId = Guid.NewGuid(),
            Name = dto.Name
        };

        await repo.AddAsync(entity, ct);
        await _uow.SaveAsync(ct);
        return entity.CategoryId;
    }

    public async Task UpdateAsync(CategoryUpdateDto dto, CancellationToken ct)
    {
        var repo = _uow.Repo<Category>();
        var entity = await repo.GetAsync(dto.CategoryId, ct)
                    ?? throw new KeyNotFoundException("Category not found");

        entity.Name = dto.Name;
        repo.Update(entity);
        await _uow.SaveAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var repo = _uow.Repo<Category>();
        var entity = await repo.GetAsync(id, ct)
                    ?? throw new KeyNotFoundException("Category not found");

        repo.Remove(entity);
        await _uow.SaveAsync(ct);
    }

    public async Task<IEnumerable<CategoryView>> GetAllAsync(CancellationToken ct)
    {
        var list = await _uow.Repo<Category>()
            .Query().OrderBy(x => x.Name).ToListAsync(ct);

        return list.Select(c => _mapper.Map<CategoryView>(c));
    }
}