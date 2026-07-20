using FlashShop.Notification.Api.Data;
using FlashShop.Notification.Api.DTOs.Requests;
using FlashShop.Notification.Api.DTOs.Responses;
using FlashShop.Notification.Api.Entities;
using FlashShop.Shared.Common;
using FlashShop.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace FlashShop.Notification.Api.Services;

public class NotificationService : INotificationService
{
    private readonly NotificationDbContext _dbContext;
    private readonly IEmailService _emailService;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(NotificationDbContext dbContext, IEmailService emailService, ILogger<NotificationService> logger)
    {
        _dbContext = dbContext;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task SendNotificationAsync(string templateKey, string recipientEmail, Dictionary<string, string> placeholders)
    {
        var template = await _dbContext.NotificationTemplates
            .FirstOrDefaultAsync(t => t.Key == templateKey && t.IsActive);

        if (template == null)
        {
            _logger.LogWarning("Template {Key} not found or inactive", templateKey);
            return;
        }

        var subject = ReplacePlaceholders(template.Subject, placeholders);
        var body = ReplacePlaceholders(template.Body, placeholders);

        var isSent = await _emailService.SendEmailAsync(recipientEmail, subject, body);

        var log = new NotificationLog
        {
            Id = Guid.NewGuid(),
            RecipientEmail = recipientEmail,
            Subject = subject,
            Body = body,
            TemplateKey = templateKey,
            IsSent = isSent,
            ErrorMessage = isSent ? null : "Failed to send email"
        };

        _dbContext.NotificationLogs.Add(log);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<PagedResult<NotificationLogResponse>> GetLogsAsync(int page, int pageSize)
    {
        var query = _dbContext.NotificationLogs.OrderByDescending(l => l.CreatedAt);
        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(l => new NotificationLogResponse
            {
                Id = l.Id,
                RecipientEmail = l.RecipientEmail,
                Subject = l.Subject,
                TemplateKey = l.TemplateKey,
                IsSent = l.IsSent,
                ErrorMessage = l.ErrorMessage,
                CreatedAt = l.CreatedAt
            })
            .ToListAsync();

        return new PagedResult<NotificationLogResponse>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<List<NotificationTemplateResponse>> GetTemplatesAsync()
    {
        return await _dbContext.NotificationTemplates
            .OrderBy(t => t.Key)
            .Select(t => new NotificationTemplateResponse
            {
                Id = t.Id,
                Key = t.Key,
                Subject = t.Subject,
                Body = t.Body,
                IsActive = t.IsActive,
                UpdatedAt = t.UpdatedAt
            })
            .ToListAsync();
    }

    public async Task<NotificationTemplateResponse> GetTemplateByKeyAsync(string key)
    {
        var template = await _dbContext.NotificationTemplates
            .FirstOrDefaultAsync(t => t.Key == key)
            ?? throw new NotFoundException($"Template '{key}' not found.");

        return new NotificationTemplateResponse
        {
            Id = template.Id,
            Key = template.Key,
            Subject = template.Subject,
            Body = template.Body,
            IsActive = template.IsActive,
            UpdatedAt = template.UpdatedAt
        };
    }

    public async Task<NotificationTemplateResponse> UpdateTemplateAsync(string key, UpdateTemplateRequest request)
    {
        var template = await _dbContext.NotificationTemplates
            .FirstOrDefaultAsync(t => t.Key == key)
            ?? throw new NotFoundException($"Template '{key}' not found.");

        template.Subject = request.Subject;
        template.Body = request.Body;
        template.IsActive = request.IsActive;
        template.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        return new NotificationTemplateResponse
        {
            Id = template.Id,
            Key = template.Key,
            Subject = template.Subject,
            Body = template.Body,
            IsActive = template.IsActive,
            UpdatedAt = template.UpdatedAt
        };
    }

    private static string ReplacePlaceholders(string text, Dictionary<string, string> placeholders)
    {
        foreach (var (key, value) in placeholders)
        {
            text = text.Replace($"{{{{{key}}}}}", value);
        }
        return text;
    }
}
