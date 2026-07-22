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
            .ToListAsync();

        var formattedLogs = logs.Select(n =>
        {
            var title = n.Subject ?? "Thông Báo Hệ Thống";
            var match = System.Text.RegularExpressions.Regex.Match(title + " " + (n.Body ?? ""), @"FS-[A-Z0-9-]+");
            var orderNum = match.Success ? match.Value : "";

            if (title.Contains("is awaiting payment", StringComparison.OrdinalIgnoreCase) || title.Contains("đã đặt thành công", StringComparison.OrdinalIgnoreCase))
            {
                title = string.IsNullOrEmpty(orderNum) ? "Đơn hàng đã đặt thành công" : $"Đơn hàng #{orderNum} đã đặt thành công";
            }
            else if (title.Contains("Payment confirmed", StringComparison.OrdinalIgnoreCase) || title.Contains("thanh toán thành công", StringComparison.OrdinalIgnoreCase))
            {
                title = string.IsNullOrEmpty(orderNum) ? "Xác nhận thanh toán thành công" : $"Đơn hàng #{orderNum} đã thanh toán thành công";
            }
            else if (title.Contains("has been cancelled", StringComparison.OrdinalIgnoreCase) || title.Contains("Order Cancelled", StringComparison.OrdinalIgnoreCase) || title.Contains("đã bị hủy", StringComparison.OrdinalIgnoreCase))
            {
                title = string.IsNullOrEmpty(orderNum) ? "Đơn hàng đã bị hủy" : $"Đơn hàng #{orderNum} đã bị hủy";
            }
            else if (title.Contains("could not be fulfilled", StringComparison.OrdinalIgnoreCase) || title.Contains("OutOfStock", StringComparison.OrdinalIgnoreCase))
            {
                title = string.IsNullOrEmpty(orderNum) ? "Đơn hàng chưa thể xử lý" : $"Đơn hàng #{orderNum} chưa thể xử lý do hết hàng";
            }

            var body = n.Body ?? "";
            body = System.Text.RegularExpressions.Regex.Replace(body, "<.*?>", " ");
            
            body = body.Replace("Order Awaiting Payment", "Đơn hàng đã đặt thành công.")
                       .Replace("Payment Confirmed", "Thanh toán thành công.")
                       .Replace("Order Cancelled", "Đơn hàng đã bị hủy.")
                       .Replace("Order Could Not Be Fulfilled", "Đơn hàng không thể xử lý.")
                       .Replace("Dear Customer,", "Kính gửi quý khách,")
                       .Replace("has been confirmed and is awaiting payment.", "đã được hệ thống xác nhận và đang được chuẩn bị.")
                       .Replace("has been confirmed.", "đã được xác nhận thanh toán thành công.")
                       .Replace("has been cancelled.", "đã bị hủy thành công.")
                       .Replace("could not be fulfilled due to insufficient stock.", "chưa thể thực hiện do sản phẩm tạm thời hết hàng.")
                       .Replace("Your order is now being processed for shipping.", "Đơn hàng của bạn đang được chuẩn bị giao.")
                       .Replace("Total Amount:", "Tổng tiền:")
                       .Replace("Please complete payment before:", "Hạn xử lý:")
                       .Replace("Best regards, FlashShop Team", "")
                       .Replace("Best regards,", "")
                       .Replace("FlashShop Team", "");

            body = System.Text.RegularExpressions.Regex.Replace(body, @"Your order\s+(FS-[A-Z0-9-]+)\s+has been cancelled", "Đơn hàng $1 đã bị hủy");
            body = System.Text.RegularExpressions.Regex.Replace(body, @"Your order\s+(FS-[A-Z0-9-]+)\s+has been confirmed", "Đơn hàng $1 đã được xác nhận");
            body = System.Text.RegularExpressions.Regex.Replace(body, @"Your order\s+(FS-[A-Z0-9-]+)\s+could not be fulfilled", "Đơn hàng $1 chưa thể thực hiện");
            body = System.Text.RegularExpressions.Regex.Replace(body, @"Your payment for order\s+(FS-[A-Z0-9-]+)", "Thanh toán cho đơn hàng $1");

            body = System.Text.RegularExpressions.Regex.Replace(body, @"\s+", " ").Trim();

            return new
            {
                id = n.Id.ToString(),
                title = title,
                message = body,
                timestamp = n.CreatedAt.ToString("o"),
                type = n.TemplateKey == "SystemPush" ? "flash_sale" : "order",
                isRead = false
            };
        });

        return Ok(ApiResponse<object>.SuccessResponse(formattedLogs));
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
