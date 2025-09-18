using System.Text.Json.Serialization;

namespace ApplicationTest.Dtos;

public class HostCreateResponse
{
    [JsonPropertyName("code")]    public string Code { get; set; } = default!;
    [JsonPropertyName("status")]  public string Status { get; set; } = default!;
    [JsonPropertyName("message")] public string? Message { get; set; }
    [JsonPropertyName("data")]    public HostProductDto Data { get; set; } = default!;
}
