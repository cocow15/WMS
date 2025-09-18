using System.ComponentModel.DataAnnotations;

namespace ApplicationTest.Entities;

public class ExternalAuthToken
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();

    [Required] public Guid UserId { get; set; }

    [Required, MaxLength(200)]
    public string Username { get; set; } = default!;

    [Required] public string Token { get; set; } = default!;

    public DateTimeOffset IssuedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? ExpiresAt { get; set; }
    public string? RawResponse { get; set; }

    public User User { get; set; } = default!;
}