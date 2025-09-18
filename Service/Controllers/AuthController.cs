using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ApplicationTest.Common;
using ApplicationTest.Dtos;
using ApplicationTest.Services;

namespace ApplicationTest.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;
    public AuthController(IAuthService auth) => _auth = auth;

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequest req, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            var errs = ModelState.Values.SelectMany(v => v.Errors)
                .Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage) ? "Invalid field" : e.ErrorMessage)
                .ToList();
            return BadRequest(BaseResponse.ToResponse(400, false, null, errs));
        }

        try
        {
            var userId = await _auth.RegisterAsync(req, ct);
            var data = new { user_id = userId, username = req.Username, email = req.Email, role = req.Role ?? "User" };
            return StatusCode(201, BaseResponse.ToResponse(201, true, data, null));
        }
        catch (InvalidOperationException ex) // dupe username/email
        {
            return Conflict(BaseResponse.ToResponse(409, false, null, new() { ex.Message }));
        }
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest req, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(BaseResponse.ToResponse(400, false, null, new() { "Invalid payload" }));

        try
        {
            var token = await _auth.LoginAsync(req, ct);
            return Ok(BaseResponse.ToResponse(200, true, new { token }, null));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(BaseResponse.ToResponse(401, false, null, new() { ex.Message }));
        }
    }
}