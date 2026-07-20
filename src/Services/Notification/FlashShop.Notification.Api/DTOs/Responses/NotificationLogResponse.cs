namespace FlashShop.Notification.Api.DTOs.Responses;

public class NotificationLogResponse
{
    public Guid Id { get; set; }
    public string RecipientEmail { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string TemplateKey { get; set; } = string.Empty;
    public bool IsSent { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; }
}
