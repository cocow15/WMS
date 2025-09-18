using System.Text.Json.Serialization;

namespace ApplicationTest.Dtos;

public class HostEnvelope<T>
{
    [JsonPropertyName("app_name")]   public string? AppName { get; set; }
    [JsonPropertyName("version")]    public string? Version { get; set; }
    [JsonPropertyName("build")]      public string? Build { get; set; }
    [JsonPropertyName("response")]   public HostResponse<T> Response { get; set; } = default!;
    [JsonPropertyName("message_en")] public string? MessageEn { get; set; }
    [JsonPropertyName("message_id")] public string? MessageId { get; set; }
}

public class HostResponse<T>
{
    [JsonPropertyName("code")]   public string Code { get; set; } = default!;
    [JsonPropertyName("status")] public string Status { get; set; } = default!;
    [JsonPropertyName("data")]   public T Data { get; set; } = default!;
}

public class HostProductDto
{
    [JsonPropertyName("ProductID")] public Guid ProductID { get; set; }
    [JsonPropertyName("SKU")] public string SKU { get; set; } = default!;
    [JsonPropertyName("Name")] public string Name { get; set; } = default!;
    [JsonPropertyName("Description")] public string? Description { get; set; }

    [JsonPropertyName("BrandID")] public Guid? BrandID { get; set; }
    [JsonPropertyName("Brand")] public string? Brand { get; set; }

    [JsonPropertyName("CategoryID")] public Guid? CategoryID { get; set; }
    [JsonPropertyName("Category")] public string? Category { get; set; }

    [JsonPropertyName("Status")] public bool Status { get; set; }
    [JsonPropertyName("Created_At")] public DateTimeOffset CreatedAt { get; set; }
    [JsonPropertyName("Created_By")] public string? CreatedBy { get; set; }
}

public class HostSimpleResponse
{
    [JsonPropertyName("code")]    public string Code { get; set; } = default!;
    [JsonPropertyName("status")]  public string Status { get; set; } = default!;
    [JsonPropertyName("message")] public string? Message { get; set; }
}