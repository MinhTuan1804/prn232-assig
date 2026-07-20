using FlashShop.Identity.Api.DTOs.Requests;
using FlashShop.Identity.Api.Services;
using FlashShop.Shared.Common;
using Microsoft.AspNetCore.Mvc;

namespace FlashShop.Identity.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);
        return Ok(ApiResponse<object>.SuccessResponse(result, "Registration successful."));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);
        return Ok(ApiResponse<object>.SuccessResponse(result, "Login successful."));
    }
}
