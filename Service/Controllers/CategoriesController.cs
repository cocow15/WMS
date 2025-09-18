using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ApplicationTest.Common;
using ApplicationTest.Dtos;
using ApplicationTest.Services;

namespace ApplicationTest.Controllers;

[ApiController]
[Authorize]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _svc;
    public CategoriesController(ICategoryService svc) => _svc = svc;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var data = await _svc.GetAllAsync(ct);
        return Ok(BaseResponse.ToResponse(200, true, data, null));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CategoryCreateDto dto, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest(BaseResponse.ToResponse(400, false, null, new() { "Name is required" }));

        var id = await _svc.CreateAsync(dto, ct);
        return Ok(BaseResponse.ToResponse(200, true, new { categoryId = id }, null));
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] CategoryUpdateDto dto, CancellationToken ct)
    {
        if (dto.CategoryId == Guid.Empty || string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest(BaseResponse.ToResponse(400, false, null, new() { "Invalid payload" }));

        await _svc.UpdateAsync(dto, ct);
        return Ok(BaseResponse.ToResponse(200, true, "updated", null));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _svc.DeleteAsync(id, ct);
        return Ok(BaseResponse.ToResponse(200, true, "deleted", null));
    }
}
