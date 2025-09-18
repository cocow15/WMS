using Microsoft.EntityFrameworkCore;
using ApplicationTest.Data;

namespace ApplicationTest.Repositories;

public class EfRepository<T> : IRepository<T> where T : class
{
    private readonly AppDbContext _ctx;
    public EfRepository(AppDbContext ctx) => _ctx = ctx;

    public async Task<T?> GetAsync(Guid id, CancellationToken ct = default)
        => await _ctx.Set<T>().FindAsync([id], ct);

    public async Task AddAsync(T entity, CancellationToken ct = default)
        => await _ctx.Set<T>().AddAsync(entity, ct);

    public void Update(T entity) => _ctx.Set<T>().Update(entity);
    public void Remove(T entity) => _ctx.Set<T>().Remove(entity);
    public IQueryable<T> Query() => _ctx.Set<T>().AsQueryable();
}
