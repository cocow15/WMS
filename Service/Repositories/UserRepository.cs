using ApplicationTest.Data;
using ApplicationTest.Entities;
using ApplicationTest.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ApplicationTest.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _ctx;
    public UserRepository(AppDbContext ctx) => _ctx = ctx;

    public Task<bool> ExistsByUsernameOrEmailAsync(string username, string email, CancellationToken ct)
        => _ctx.Users.AnyAsync(u => u.Username == username || u.Email == email, ct);

    public Task<User?> GetActiveByUsernameAsync(string username, CancellationToken ct)
        => _ctx.Users.FirstOrDefaultAsync(u => u.Username == username && u.IsActive, ct);

    public Task AddAsync(User user, CancellationToken ct) => _ctx.Users.AddAsync(user, ct).AsTask();

    public Task<int> SaveChangesAsync(CancellationToken ct) => _ctx.SaveChangesAsync(ct);
}
