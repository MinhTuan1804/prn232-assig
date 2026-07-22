using FlashShop.Notification.Api.Data;
using FlashShop.Notification.Api.DTOs.Requests;
using FlashShop.Notification.Api.Entities;
using FlashShop.Notification.Api.Services;
using FlashShop.Shared.Common;
using FlashShop.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlashShop.Notification.Api.Controllers;

[ApiController]
[Route("api/notifications")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly NotificationDbContext _dbContext;

    public NotificationsController(INotificationService notificationService, NotificationDbContext dbContext)
    {
        _notificationService = notificationService;
        _dbContext = dbContext;
    }

    [HttpGet("logs")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> GetLogs([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _notificationService.GetLogsAsync(page, pageSize);
        return Ok(ApiResponse<object>.SuccessResponse(result));
    }

    [HttpGet("my-notifications")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPublicNotifications()
    {
        var logs = await _dbContext.NotificationLogs
            .OrderByDescending(n => n.CreatedAt)
            .Take(30)
            .Select(n => new
            {
                id = n.Id.ToString(),
                title = n.Subject,
                message = n.Body,
                timestamp = n.CreatedAt.ToString("o"),
                type = n.TemplateKey == "SystemPush" ? "flash_sale" : "order",
                isRead = false
            })
            .ToListAsync();

        return Ok(ApiResponse<object>.SuccessResponse(logs));
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
    public async Task<IActionResult> PushTest([FromBody] PushTestRequest? request)
    {
        var title = string.IsNullOrWhiteSpace(request?.Title) ? "⚡ BÁO ĐỘNG SALE SỐC 2026" : request.Title.Trim();
        var message = string.IsNullOrWhiteSpace(request?.Message) ? "Chuột Gaming Razer Viper V3 Pro 54g siêu nhẹ đang giảm giá 30%!" : request.Message.Trim();

        var log = new NotificationLog
        {
            Id = Guid.NewGuid(),
            RecipientEmail = "all@flashshop.com",
            Subject = title,
            Body = message,
            TemplateKey = "SystemPush",
            IsSent = true,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.NotificationLogs.Add(log);
        await _dbContext.SaveChangesAsync();

        var notification = new
        {
            id = log.Id.ToString(),
            title = log.Subject,
            message = log.Body,
            timestamp = log.CreatedAt.ToString("o"),
            type = "flash_sale",
            isRead = false
        };

        return Ok(ApiResponse<object>.SuccessResponse(notification, "Push notification sent successfully."));
    }
}

public class PushTestRequest
{
    public string? Title { get; set; }
    public string? Message { get; set; }
}
