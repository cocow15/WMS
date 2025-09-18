using System.ComponentModel.DataAnnotations;

namespace ApplicationTest.Entities;

public class Category
{
    [Key] public Guid CategoryId { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = default!;

    public ICollection<Product> Products { get; set; } = new List<Product>();
}
