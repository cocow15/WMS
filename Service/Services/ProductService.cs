using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using ApplicationTest.Dtos;
using ApplicationTest.Entities;
using ApplicationTest.Repositories;

namespace ApplicationTest.Services;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public ProductService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<(IEnumerable<ProductView> data, int total)> GetListAsync(ProductListRequest req, CancellationToken ct)
    {
        IQueryable<Product> q = _uow.Repo<Product>().Query();

        if ( req.filter.guid.HasValue)
            q = q.Where(p => p.ProductId == req.filter.guid);

        if ( req.filter.category_id is not null && req.filter.category_id.Any())
            q = q.Where(p => p.CategoryId != null && req.filter.category_id.Contains(p.CategoryId.Value));

        if ( !string.IsNullOrWhiteSpace(req.filter.name))
            q = q.Where(p => EF.Functions.ILike(p.Name, $"%{req.filter.name}%"));

        if ( !string.IsNullOrWhiteSpace(req.filter.status))
            q = q.Where(p => p.Status == (req.filter.status.ToLower() == "active"));

        q = (req.order?.ToLower(), req.sort?.ToUpper()) switch
        {
            ("created_at", "ASC")  => q.OrderBy(p => p.CreatedAt),
            ("created_at", "DESC") => q.OrderByDescending(p => p.CreatedAt),
            ("name", "ASC")        => q.OrderBy(p => p.Name),
            ("name", "DESC")       => q.OrderByDescending(p => p.Name),
            _                      => q.OrderByDescending(p => p.CreatedAt)
        };

        q = q.Include(p => p.Brand)
             .Include(p => p.Category);

        var total = await q.CountAsync(ct);

        var data = await q.Skip((req.page - 1) * req.limit)
                          .Take(req.limit)
                          .ProjectTo<ProductView>(_mapper.ConfigurationProvider)
                          .ToListAsync(ct);

        return (data, total);
    }

    public async Task<ProductView?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var product = await _uow.Repo<Product>().Query()
            .Include(p => p.Brand)
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.ProductId == id, ct);

        if (product == null) return null;

        return new ProductView(
            product.ProductId,
            product.Sku,
            product.Name,
            product.Description,
            product.BrandId,
            product.Brand?.Name,
            product.CategoryId,
            product.Category?.Name,
            product.Status,
            product.CreatedAt
        );
    }

    public async Task<Guid> CreateAsync(ProductCreateDto dto, Guid currentUserId, CancellationToken ct)
    {
        // ensure Brand
        Guid? brandId = dto.BrandId;
        if (brandId == null && !string.IsNullOrWhiteSpace(dto.Brand))
        {
            var bRepo = _uow.Repo<Brand>();
            var b = await bRepo.Query()
                .FirstOrDefaultAsync(x => x.Name.ToLower() == dto.Brand!.ToLower(), ct);

            if (b is null)
            {
                b = new Brand { BrandId = Guid.NewGuid(), Name = dto.Brand! };
                await bRepo.AddAsync(b, ct);
            }
            brandId = b.BrandId;
        }

        // ensure Category
        Guid? categoryId = dto.CategoryId;
        if (categoryId == null && !string.IsNullOrWhiteSpace(dto.Category))
        {
            var cRepo = _uow.Repo<Category>();
            var c = await cRepo.Query()
                .FirstOrDefaultAsync(x => x.Name.ToLower() == dto.Category!.ToLower(), ct);

            if (c is null)
            {
                c = new Category { CategoryId = Guid.NewGuid(), Name = dto.Category! };
                await cRepo.AddAsync(c, ct);
            }
            categoryId = c.CategoryId;
        }

        var entity = new Product
        {
            ProductId = Guid.NewGuid(),
            Sku = dto.Sku,
            Name = dto.Name,
            Description = dto.Description,
            BrandId = brandId,
            CategoryId = categoryId,
            Status = dto.Status,
            UserId = currentUserId,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        await _uow.Repo<Product>().AddAsync(entity, ct);
        await _uow.SaveAsync(ct);
        return entity.ProductId;
    }

    public async Task UpdateAsync(ProductUpdateDto dto, CancellationToken ct)
    {
        await using var tx = await _uow.BeginTransactionAsync(ct);
        try
        {
            var repo = _uow.Repo<Product>();
            var entity = await repo.GetAsync(dto.ProductId, ct)
                        ?? throw new KeyNotFoundException("Product not found");

            Guid? brandId = entity.BrandId;
            if (dto.BrandId.HasValue)
            {
                brandId = dto.BrandId.Value;
            }
            else if (!string.IsNullOrWhiteSpace(dto.Brand))
            {
                var bRepo = _uow.Repo<Brand>();
                var b = await bRepo.Query()
                    .FirstOrDefaultAsync(x => x.Name.ToLower() == dto.Brand!.ToLower(), ct);

                if (b is null)
                {
                    b = new Brand { BrandId = Guid.NewGuid(), Name = dto.Brand! };
                    await bRepo.AddAsync(b, ct);
                }
                brandId = b.BrandId;
            }

            Guid? categoryId = entity.CategoryId;

            if (dto.CategoryId.HasValue)
            {
                categoryId = dto.CategoryId.Value;
            }
            else if (!string.IsNullOrWhiteSpace(dto.Category))
            {
                var cRepo = _uow.Repo<Category>();
                var c = await cRepo.Query()
                    .FirstOrDefaultAsync(x => x.Name.ToLower() == dto.Category!.ToLower(), ct);

                if (c is null)
                {
                    c = new Category { CategoryId = Guid.NewGuid(), Name = dto.Category! };
                    await cRepo.AddAsync(c, ct);
                }
                categoryId = c.CategoryId;
            }

            entity.Sku = dto.Sku;
            entity.Name = dto.Name;
            entity.Description = dto.Description;
            entity.BrandId = brandId;
            entity.CategoryId = categoryId;
            entity.Status = dto.Status;
            entity.UpdatedAt = DateTimeOffset.UtcNow;


            var affected = await _uow.SaveAsync(ct);
            if (affected <= 0)
                throw new InvalidOperationException("No rows were updated.");

            await tx.CommitAsync(ct);
        }
        catch
        {
            await tx.RollbackAsync(ct);
            throw;
        }
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var repo = _uow.Repo<Product>();
        var entity = await repo.GetAsync(id, ct)
                     ?? throw new KeyNotFoundException("Product not found");

        repo.Remove(entity);
        await _uow.SaveAsync(ct);
    }
}