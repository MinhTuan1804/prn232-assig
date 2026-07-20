using System.ComponentModel.DataAnnotations;

namespace FlashShop.Notification.Api.DTOs.Requests;

public class UpdateTemplateRequest
{
    [Required]
    public string Subject { get; set; } = string.Empty;

    [Required]
    public string Body { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}
