using FlashShop.Notification.Api.DTOs.Requests;
using FlashShop.Notification.Api.DTOs.Responses;
using FlashShop.Shared.Common;

namespace FlashShop.Notification.Api.Services;

/// <summary>
/// Resolves templates, substitutes event data, sends email, and records the
/// delivery attempt for operational history.
/// </summary>
public interface INotificationService
{
    /// <summary>Sends one templated notification to its recipient.</summary>
    Task SendNotificationAsync(string templateKey, string recipientEmail, Dictionary<string, string> placeholders);

    /// <summary>Returns the paged delivery log for administration.</summary>
    Task<PagedResult<NotificationLogResponse>> GetLogsAsync(int page, int pageSize);

    /// <summary>Lists all notification templates.</summary>
    Task<List<NotificationTemplateResponse>> GetTemplatesAsync();

    /// <summary>Gets the template selected by an integration event key.</summary>
    Task<NotificationTemplateResponse> GetTemplateByKeyAsync(string key);

    /// <summary>Updates administrator-editable template content.</summary>
    Task<NotificationTemplateResponse> UpdateTemplateAsync(string key, UpdateTemplateRequest request);
}
