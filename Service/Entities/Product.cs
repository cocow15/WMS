using System.ComponentModel.DataAnnotations;

namespace ApplicationTest.Entities;

public class Product
{
    [Key] public Guid ProductId { get; set; }

    [Required, MaxLength(50)]
    public string Sku { get; set; } = default!;

    [Required, MaxLength(200)]
    public string Name { get; set; } = default!;

    public string? Description { get; set; }

    public Guid? BrandId { get; set; }
    public Brand? Brand { get; set; }

    public Guid? CategoryId { get; set; }
    public Category? Category { get; set; }

    public Guid? UserId { get; set; }
    public User? User { get; set; }

    public bool Status { get; set; } = true;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}