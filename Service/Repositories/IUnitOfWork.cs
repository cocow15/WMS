using Microsoft.EntityFrameworkCore.Storage;

namespace ApplicationTest.Repositories;

public interface IUnitOfWork
{
    IRepository<T> Repo<T>() where T : class;
    Task<int> SaveAsync(CancellationToken ct = default);
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken ct = default);
}
