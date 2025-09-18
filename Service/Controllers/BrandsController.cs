using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ApplicationTest.Common;
using ApplicationTest.Dtos;
using ApplicationTest.Services;

namespace ApplicationTest.Controllers;

[ApiController]
[Authorize]
[Route("api/brands")]
public class BrandsController : ControllerBase
{
    private readonly IBrandService _svc;
    public BrandsController(IBrandService svc) => _svc = svc;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => Ok(BaseResponse.ToResponse(200, true, await _svc.GetAllAsync(ct), null));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] BrandCreateDto dto, CancellationToken ct)
        => Ok(BaseResponse.ToResponse(200, true, new { brandId = await _svc.CreateAsync(dto, ct) }, null));

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] BrandUpdateDto dto, CancellationToken ct)
    { await _svc.UpdateAsync(dto, ct); return Ok(BaseResponse.ToResponse(200, true, "updated", null)); }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    { await _svc.DeleteAsync(id, ct); return Ok(BaseResponse.ToResponse(200, true, "deleted", null)); }
}
