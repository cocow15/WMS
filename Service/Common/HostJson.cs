using System.Text.Json;

namespace ApplicationTest.Common;

public static class HostJson
{
    private static readonly JsonSerializerOptions _opts = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static T? TryUnwrapAndDeserialize<T>(string content)
    {
        try
        {
            using var doc = JsonDocument.Parse(content);
            if (doc.RootElement.ValueKind == JsonValueKind.Object &&
                doc.RootElement.TryGetProperty("d", out var dProp) &&
                dProp.ValueKind == JsonValueKind.String)
            {
                var inner = dProp.GetString();
                if (!string.IsNullOrWhiteSpace(inner))
                    return JsonSerializer.Deserialize<T>(inner, _opts);
            }

            return JsonSerializer.Deserialize<T>(content, _opts);
        }
        catch
        {
            return default;
        }
    }
}
