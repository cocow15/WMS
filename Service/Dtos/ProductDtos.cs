using System.ComponentModel.DataAnnotations;

namespace ApplicationTest.Dtos;

public class ProductFilter
{
    //public bool set_guid { get; set; }
    public Guid? guid { get; set; }

    //public bool set_category_id { get; set; }
    public IEnumerable<Guid>? category_id { get; set; }

    //public bool set_name { get; set; }
    public string? name { get; set; }

    //public bool set_status { get; set; }
    public string? status { get; set; }
}

public class ProductListRequest
{
    [Required] public ProductFilter filter { get; set; } = new();
    [Range(1, int.MaxValue)] public int limit { get; set; } = 10;
    [Range(1, int.MaxValue)] public int page { get; set; } = 1;
    public string order { get; set; } = "created_at";
    public string sort { get; set; } = "DESC";
}

public class ProductCreateDto
{
    [Required, MaxLength(50)] public string Sku { get; set; } = default!;
    [Required, MaxLength(200)] public string Name { get; set; } = default!;
    public string? Description { get; set; }

    public Guid? BrandId { get; set; }
    [MaxLength(100)] public string? Brand { get; set; }

    public Guid? CategoryId { get; set; }
    [MaxLength(100)] public string? Category { get; set; }

    public bool Status { get; set; } = true;
}

public class ProductUpdateDto : ProductCreateDto
{
    [Required] public Guid ProductId { get; set; }
}

public record ProductView(
    Guid ProductId, string Sku, string Name, string? Description,
    Guid? BrandId, string? Brand, Guid? CategoryId, string? Category,
    bool Status, DateTimeOffset CreatedAt
);
