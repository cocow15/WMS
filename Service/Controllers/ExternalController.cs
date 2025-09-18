using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ApplicationTest.Common;
using ApplicationTest.Dtos;
using ApplicationTest.Services;

namespace ApplicationTest.Controllers;

[ApiController]
[Route("api/external")]
[Authorize]
public class ExternalController : ControllerBase
{
    private readonly IExternalAuthService _svc;
    public ExternalController(IExternalAuthService svc) => _svc = svc;

    [HttpPost("login-and-save")]
    public async Task<IActionResult> LoginAndSave([FromBody] ExternalLoginRequest req, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            var errs = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(BaseResponse.ToResponse(400, false, null, errs));
        }

        var (saved, exp) = await _svc.LoginAndSaveAsync(req, ct);
        if (!saved) return BadRequest(BaseResponse.ToResponse(400, false, null, new() { "Token not found in response" }));
        return Ok(BaseResponse.ToResponse(200, true, new { saved = true, expires_at = exp }, null));
    }
}