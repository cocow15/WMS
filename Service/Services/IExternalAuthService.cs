using ApplicationTest.Dtos;

namespace ApplicationTest.Services;

public interface IExternalAuthService
{
    Task<(bool saved, DateTimeOffset? expiresAt)> LoginAndSaveAsync(ExternalLoginRequest req, CancellationToken ct);
    Task<string?> GetLatestTokenAsync(CancellationToken ct);
}