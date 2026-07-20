namespace FlashShop.Notification.Api.DTOs.Responses;

public class NotificationTemplateResponse
{
    public Guid Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime UpdatedAt { get; set; }
}
