namespace ApplicationTest.Common;

public class BaseResponse
{
    public int Code { get; set; }
    public bool Success { get; set; }
    public object? Data { get; set; }
    public object? Page { get; set; }
    public List<string>? Errors { get; set; }

    public static BaseResponse ToResponse(int code, bool success, dynamic? data, List<string>? errors)
        => new() { Code = code, Success = success, Data = data, Errors = errors };

    public static BaseResponse ToResponsePagination(int code, bool success, object? data, object? page, List<string>? errors)
        => new() { Code = code, Success = success, Data = data, Page = page, Errors = errors };
}
public record PageMeta(int page, int limit, int total, int total_pages);
