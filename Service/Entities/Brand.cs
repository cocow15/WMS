using System.ComponentModel.DataAnnotations;

namespace ApplicationTest.Entities;

public class Brand
{
    [Key] public Guid BrandId { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = default!;

    public ICollection<Product> Products { get; set; } = new List<Product>();
}
