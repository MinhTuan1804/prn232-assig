using FlashShop.Notification.Api.DTOs.Requests;
using FlashShop.Notification.Api.DTOs.Responses;
using FlashShop.Shared.Common;

namespace FlashShop.Notification.Api.Services;

public interface INotificationService
{
    Task SendNotificationAsync(string templateKey, string recipientEmail, Dictionary<string, string> placeholders);
    Task<PagedResult<NotificationLogResponse>> GetLogsAsync(int page, int pageSize);
    Task<List<NotificationTemplateResponse>> GetTemplatesAsync();
    Task<NotificationTemplateResponse> GetTemplateByKeyAsync(string key);
    Task<NotificationTemplateResponse> UpdateTemplateAsync(string key, UpdateTemplateRequest request);
}
