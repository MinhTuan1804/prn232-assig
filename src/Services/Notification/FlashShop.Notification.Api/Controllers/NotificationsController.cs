using FlashShop.Notification.Api.DTOs.Requests;
using FlashShop.Notification.Api.Services;
using FlashShop.Shared.Common;
using FlashShop.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlashShop.Notification.Api.Controllers;

[ApiController]
[Route("api/notifications")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet("logs")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> GetLogs([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _notificationService.GetLogsAsync(page, pageSize);
        return Ok(ApiResponse<object>.SuccessResponse(result));
    }

    [HttpGet("templates")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> GetTemplates()
    {
        var result = await _notificationService.GetTemplatesAsync();
        return Ok(ApiResponse<object>.SuccessResponse(result));
    }

    [HttpGet("templates/{key}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> GetTemplate(string key)
    {
        var result = await _notificationService.GetTemplateByKeyAsync(key);
        return Ok(ApiResponse<object>.SuccessResponse(result));
    }

    [HttpPut("templates/{key}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> UpdateTemplate(string key, [FromBody] UpdateTemplateRequest request)
    {
        var result = await _notificationService.UpdateTemplateAsync(key, request);
        return Ok(ApiResponse<object>.SuccessResponse(result, "Template updated."));
    }

    [HttpPost("push-test")]
    [AllowAnonymous]
    public IActionResult PushTest([FromBody] PushTestRequest? request)
    {
        var title = request?.Title ?? "⚡ BÁO ĐỘNG SALE SỐC 2026";
        var message = request?.Message ?? "Chuột Gaming Razer Viper V3 Pro 54g siêu nhẹ đang giảm giá 30%!";

        var notification = new
        {
            Id = Guid.NewGuid(),
            Title = title,
            Message = message,
            Timestamp = DateTime.UtcNow.ToString("o"),
            Type = "flash_sale",
            Status = "Pushed"
        };

        return Ok(ApiResponse<object>.SuccessResponse(notification, "Push notification test message sent successfully."));
    }
}

public class PushTestRequest
{
    public string? Title { get; set; }
    public string? Message { get; set; }
}
