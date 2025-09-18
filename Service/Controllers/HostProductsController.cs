using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ApplicationTest.Common; 
using ApplicationTest.Dtos;
using ApplicationTest.Services;

namespace ApplicationTest.Controllers;

[ApiController]
[Route("api/host/products")]
[Authorize]
public class HostProductsController : ControllerBase
{
    private readonly IHostProductService _svc;
    public HostProductsController(IHostProductService svc) => _svc = svc;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ProductCreateRequest req, CancellationToken ct)
    {
        var (code, content, ctType) = await _svc.CreateAsync(req, ct);

        var obj = HostJson.TryUnwrapAndDeserialize<HostCreateResponse>(content);
        if (obj is null)
        {
            return StatusCode(code, BaseResponse.ToResponse(code, false, new { raw = content }, new() { "Failed to parse host response" }));
        }

        var success = (obj.Code == "00" || obj.Status.Equals("success", StringComparison.OrdinalIgnoreCase));
        return StatusCode(code, BaseResponse.ToResponse(code, success, obj, success ? null : new() { obj.Message ?? "Host error" }));
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] ProductUpdateRequest req, CancellationToken ct)
    {
        var (code, content, ctType) = await _svc.UpdateAsync(req, ct);

        // Unwrap {"d":"{...}"} dan deserialize ke object kuat-tipe
        var obj = HostJson.TryUnwrapAndDeserialize<HostCreateResponse>(content);
        if (obj is null)
        {
            return StatusCode(code,
                BaseResponse.ToResponse(code, false, new { raw = content },
                new() { "Failed to parse host response" }));
        }

        var success = (obj.Code == "00" ||
                    obj.Status.Equals("success", StringComparison.OrdinalIgnoreCase));

        return StatusCode(code,
            BaseResponse.ToResponse(code, success, obj,
            success ? null : new() { obj.Message ?? "Host error" }));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var req = new GetHostProductReq { productId = id };

        var (code, content, _) = await _svc.DeleteAsync(req, ct);

        var obj = HostJson.TryUnwrapAndDeserialize<HostSimpleResponse>(content);
        if (obj is null)
        {
            return StatusCode(code, BaseResponse.ToResponse(
                code, false, new { raw = content }, new() { "Failed to parse host delete response" }));
        }

        var success = (obj.Code == "00" ||
                    obj.Status.Equals("success", StringComparison.OrdinalIgnoreCase));

        var data = new { id, message = obj.Message ?? (success ? "Deleted" : "Failed") };

        return StatusCode(code, BaseResponse.ToResponse(
            code, success, data, success ? null : new() { obj.Message ?? "Host error" }));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var req = new GetHostProductReq { productId = id };
        var (code, content, _) = await _svc.GetByIdAsync(req, ct);
        var env = HostJson.TryUnwrapAndDeserialize<HostEnvelope<HostProductDto>>(content);
        if (env is null)
        {
            return StatusCode(code, BaseResponse.ToResponse(
                code, false, new { raw = content }, new() { "Failed to parse host get-by-id response" }));
        }

        var success = env.Response.Code == "00" ||
                    env.Response.Status.Equals("success", StringComparison.OrdinalIgnoreCase);

        return StatusCode(code, BaseResponse.ToResponse(
            code, success, env.Response.Data, success ? null : new() { env.MessageEn ?? "Host error" }));
    }

    [HttpGet("list")]
    public async Task<IActionResult> List(CancellationToken ct)
    {
        var (code, content, ctType) = await _svc.ListAsync(ct);
        return new ContentResult { Content = content, ContentType = ctType, StatusCode = code };
    }
}