using System.ComponentModel.DataAnnotations;

namespace ApplicationTest.Dtos;

public class ExternalLoginRequest
{
    [Required, MaxLength(200)] public string username { get; set; } = default!;
    [Required, MaxLength(200)] public string password { get; set; } = default!;
}

public class ProductCreateRequest
{
    [Required, MaxLength(50)]  public string sku { get; set; } = default!;
    [Required, MaxLength(200)] public string name { get; set; } = default!;
    public string? description { get; set; }
    public Guid? brandId { get; set; }
    public string? brand { get; set; }
    public Guid? categoryId { get; set; }
    public string? category { get; set; }
    public bool status { get; set; } = true;
    public string? createdBy { get; set; }
}

public class ProductUpdateRequest : ProductCreateRequest
{
    [Required] public Guid id { get; set; } 
}

public class GetHostProductReq
{
    [Required] public Guid productId { get; set; }
}