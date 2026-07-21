using FlashShop.Identity.Api.DTOs.Requests;
using FlashShop.Identity.Api.Services;
using FlashShop.Shared.Common;
using FlashShop.Shared.Constants;
using FlashShop.Shared.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlashShop.Identity.Api.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("profile")]
    [HttpGet("me")]
    public async Task<IActionResult> GetProfile()
    {
        var userId = User.GetUserId();
        var result = await _userService.GetByIdAsync(userId);
        return Ok(ApiResponse<object>.SuccessResponse(result));
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _userService.GetByIdAsync(id);
        return Ok(ApiResponse<object>.SuccessResponse(result));
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        var userId = User.GetUserId();
        var result = await _userService.UpdateProfileAsync(userId, request);
        return Ok(ApiResponse<object>.SuccessResponse(result, "Profile updated."));
    }

    [HttpGet]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _userService.GetAllUsersAsync(page, pageSize);
        return Ok(ApiResponse<object>.SuccessResponse(result));
    }

    [HttpPut("{id:guid}/toggle-active")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> ToggleActive(Guid id)
    {
        var result = await _userService.ToggleActiveAsync(id);
        return Ok(ApiResponse<object>.SuccessResponse(result));
    }

    [HttpPut("{id:guid}/role")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> ChangeRole(Guid id, [FromQuery] string role)
    {
        var result = await _userService.ChangeRoleAsync(id, role);
        return Ok(ApiResponse<object>.SuccessResponse(result));
    }
}
