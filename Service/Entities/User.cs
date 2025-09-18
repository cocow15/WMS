using System.ComponentModel.DataAnnotations;

namespace ApplicationTest.Entities;

public class User
{
    [Key] public Guid UserId { get; set; }

    [Required, MaxLength(50)]
    public string Role { get; set; } = default!;

    [Required, MaxLength(200)]
    public string Name { get; set; } = default!;

    [Required, MaxLength(200)]
    public string Email { get; set; } = default!;

    [Required, MaxLength(200)]
    public string Username { get; set; } = default!;

    [Required] public string PasswordHash { get; set; } = default!;

    public bool IsActive { get; set; } = true;

    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public DateTimeOffset? TokenExpired { get; set; }

    [MaxLength(64)] public string? CreatedBy { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public ExternalAuthToken? ExternalAuthToken { get; set; }
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
