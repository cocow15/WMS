using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ApplicationTest.Common;
using ApplicationTest.Dtos;
using ApplicationTest.Services;

namespace ApplicationTest.Controllers;

[ApiController]
[Authorize]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _svc;
    public ProductsController(IProductService svc) => _svc = svc;

    [HttpPost("list")]
    public async Task<IActionResult> GetList([FromBody] ProductListRequest req, CancellationToken ct)
    {
        var (data, total) = await _svc.GetListAsync(req, ct);
        var page = new PageMeta(req.page, req.limit, total, (int)Math.Ceiling((double)total / req.limit));
        return Ok(BaseResponse.ToResponsePagination(200, true, data, page, null));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var data = await _svc.GetByIdAsync(id, ct);
        if (data is null) return NotFound(BaseResponse.ToResponse(404, false, null, new() { "Not found" }));
        return Ok(BaseResponse.ToResponse(200, true, data, null));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ProductCreateDto dto, CancellationToken ct)
    {
        var uidStr = User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(uidStr))
            return Unauthorized(BaseResponse.ToResponse(401, false, null, new() { "Missing user id claim" }));

        if (!Guid.TryParse(uidStr, out var currentUserId))
            return Unauthorized(BaseResponse.ToResponse(401, false, null, new() { "Invalid user id claim" }));

        var id = await _svc.CreateAsync(dto, currentUserId, ct);
        return Ok(BaseResponse.ToResponse(200, true, new { productId = id }, null));
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] ProductUpdateDto dto, CancellationToken ct)
    { await _svc.UpdateAsync(dto, ct); return Ok(BaseResponse.ToResponse(200, true, "updated", null)); }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    { await _svc.DeleteAsync(id, ct); return Ok(BaseResponse.ToResponse(200, true, "deleted", null)); }
}