using FlashShop.Notification.Api.DTOs.Requests;
using FlashShop.Notification.Api.Services;
using FlashShop.Shared.Common;
using FlashShop.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlashShop.Notification.Api.Controllers;

[ApiController]
[Route("api/notifications")]
[Authorize(Roles = Roles.Admin)]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet("logs")]
    public async Task<IActionResult> GetLogs([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _notificationService.GetLogsAsync(page, pageSize);
        return Ok(ApiResponse<object>.SuccessResponse(result));
    }

    [HttpGet("templates")]
    public async Task<IActionResult> GetTemplates()
    {
        var result = await _notificationService.GetTemplatesAsync();
        return Ok(ApiResponse<object>.SuccessResponse(result));
    }

    [HttpGet("templates/{key}")]
    public async Task<IActionResult> GetTemplate(string key)
    {
        var result = await _notificationService.GetTemplateByKeyAsync(key);
        return Ok(ApiResponse<object>.SuccessResponse(result));
    }

    [HttpPut("templates/{key}")]
    public async Task<IActionResult> UpdateTemplate(string key, [FromBody] UpdateTemplateRequest request)
    {
        var result = await _notificationService.UpdateTemplateAsync(key, request);
        return Ok(ApiResponse<object>.SuccessResponse(result, "Template updated."));
    }
}
