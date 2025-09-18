using ApplicationTest.Dtos;

namespace ApplicationTest.Services;

public interface IAuthService
{
    Task<Guid> RegisterAsync(RegisterRequest req, CancellationToken ct);
    Task<string> LoginAsync(LoginRequest req, CancellationToken ct);
}
