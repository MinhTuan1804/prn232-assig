namespace FlashShop.Notification.Api.Services;

/// <summary>Abstracts SMTP delivery from template and logging concerns.</summary>
public interface IEmailService
{
    /// <summary>
    /// Sends an HTML email and reports whether the SMTP provider accepted it.
    /// </summary>
    Task<bool> SendEmailAsync(string to, string subject, string htmlBody);
}
