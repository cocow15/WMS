using Microsoft.EntityFrameworkCore.Storage;
using ApplicationTest.Data;

namespace ApplicationTest.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _ctx;
    private readonly Dictionary<Type, object> _repos = new();
    public UnitOfWork(AppDbContext ctx) => _ctx = ctx;

    public IRepository<T> Repo<T>() where T : class
    {
        if (_repos.TryGetValue(typeof(T), out var r)) return (IRepository<T>)r;
        var inst = new EfRepository<T>(_ctx);
        _repos[typeof(T)] = inst;
        return inst;
    }

    public Task<int> SaveAsync(CancellationToken ct = default) => _ctx.SaveChangesAsync(ct);

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken ct = default)
        => _ctx.Database.BeginTransactionAsync(ct);
}