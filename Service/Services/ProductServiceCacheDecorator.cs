// using Microsoft.Extensions.Caching.Memory;
// using ApplicationTest.Dtos;

// namespace ApplicationTest.Services;

// public class ProductServiceCacheDecorator : IProductService
// {
//     private readonly IProductService _inner;
//     private readonly IMemoryCache _cache;

//     public ProductServiceCacheDecorator(IProductService inner, IMemoryCache cache)
//     {
//         _inner = inner;
//         _cache = cache;
//     }

//     public Task<Guid> CreateAsync(ProductCreateDto dto, Guid currentUserId, CancellationToken ct)
//         => _inner.CreateAsync(dto, currentUserId, ct);

//     public Task DeleteAsync(Guid id, CancellationToken ct)
//         => _inner.DeleteAsync(id, ct);

//     public async Task<ProductView?> GetByIdAsync(Guid id, CancellationToken ct)
//     {
//         var key = $"prod:byid:{id}";
//         return await _cache.GetOrCreateAsync(key, e =>
//         {
//             e.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
//             return _inner.GetByIdAsync(id, ct);
//         })!;
//     }

//     public async Task<(IEnumerable<ProductView> data, int total)> GetListAsync(ProductListRequest req, CancellationToken ct)
//     {
//         var key = $"prod:list:{req.page}:{req.limit}:{req.order}:{req.sort}:{req.filter.status}:{req.filter.name}";
//         return await _cache.GetOrCreateAsync(key, e =>
//         {
//             e.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(3);
//             return _inner.GetListAsync(req, ct);
//         })!;
//     }

//     public Task UpdateAsync(ProductUpdateDto dto, CancellationToken ct)
//         => _inner.UpdateAsync(dto, ct);
// }
