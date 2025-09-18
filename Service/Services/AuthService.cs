// File: Services/AuthService.cs
using ApplicationTest.Dtos;
using ApplicationTest.Entities;
using ApplicationTest.Repositories;
using ApplicationTest.Services;

namespace ApplicationTest.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly IJwtTokenProvider _jwt;

    public AuthService(IUserRepository users, IJwtTokenProvider jwt)
    {
        _users = users; _jwt = jwt;
    }

    public async Task<Guid> RegisterAsync(RegisterRequest req, CancellationToken ct)
    {
        var dupe = await _users.ExistsByUsernameOrEmailAsync(req.Username, req.Email, ct);
        if (dupe) throw new InvalidOperationException("Username or Email already exists");

        var user = new User
        {
            UserId = Guid.NewGuid(),
            Name = req.Name,
            Email = req.Email,
            Username = req.Username,
            Role = string.IsNullOrWhiteSpace(req.Role) ? "User" : req.Role!,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password),
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow,
            CreatedBy = "self-register"
        };

        await _users.AddAsync(user, ct);
        await _users.SaveChangesAsync(ct);
        return user.UserId;
    }

    public async Task<string> LoginAsync(LoginRequest req, CancellationToken ct)
    {
        var user = await _users.GetActiveByUsernameAsync(req.Username, ct);
        var ok = user is not null && BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash);
        if (!ok) throw new UnauthorizedAccessException("Invalid credentials");
        return _jwt.Generate(user!);
    }
}