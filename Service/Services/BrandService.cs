using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ApplicationTest.Dtos;
using ApplicationTest.Entities;
using ApplicationTest.Repositories;

namespace ApplicationTest.Services;

public class BrandService : IBrandService
{
    private readonly IUnitOfWork _uow; private readonly IMapper _mapper;
    public BrandService(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

    public async Task<Guid> CreateAsync(BrandCreateDto dto, CancellationToken ct)
    {
        var repo = _uow.Repo<Brand>();
        var exist = await repo.Query().FirstOrDefaultAsync(x => x.Name.ToLower() == dto.Name.ToLower(), ct);
        if (exist != null) return exist.BrandId;

        var entity = new Brand { BrandId = Guid.NewGuid(), Name = dto.Name };
        await repo.AddAsync(entity, ct);
        await _uow.SaveAsync(ct);
        return entity.BrandId;
    }
    public async Task UpdateAsync(BrandUpdateDto dto, CancellationToken ct)
    {
        var repo = _uow.Repo<Brand>();
        var entity = await repo.GetAsync(dto.BrandId, ct) ?? throw new KeyNotFoundException("Brand not found");
        entity.Name = dto.Name;
        repo.Update(entity);
        await _uow.SaveAsync(ct);
    }
    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var repo = _uow.Repo<Brand>();
        var entity = await repo.GetAsync(id, ct) ?? throw new KeyNotFoundException("Brand not found");
        repo.Remove(entity);
        await _uow.SaveAsync(ct);
    }
    public async Task<IEnumerable<BrandView>> GetAllAsync(CancellationToken ct)
    {
        var data = await _uow.Repo<Brand>().Query().OrderBy(x => x.Name).ToListAsync(ct);
        return data.Select(b => _mapper.Map<BrandView>(b));
    }
}