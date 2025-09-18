using ApplicationTest.Entities;

namespace ApplicationTest.Repositories;

public interface IUserRepository
{
    Task<bool> ExistsByUsernameOrEmailAsync(string username, string email, CancellationToken ct);
    Task<User?> GetActiveByUsernameAsync(string username, CancellationToken ct);
    Task AddAsync(User user, CancellationToken ct);
    Task<int> SaveChangesAsync(CancellationToken ct);
}