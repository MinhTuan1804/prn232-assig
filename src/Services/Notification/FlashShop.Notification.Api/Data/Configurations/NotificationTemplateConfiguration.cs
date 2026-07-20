using FlashShop.Notification.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlashShop.Notification.Api.Data.Configurations;

public class NotificationTemplateConfiguration : IEntityTypeConfiguration<NotificationTemplate>
{
    public void Configure(EntityTypeBuilder<NotificationTemplate> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Key).IsRequired().HasMaxLength(100);
        builder.Property(t => t.Subject).IsRequired().HasMaxLength(500);
        builder.Property(t => t.Body).IsRequired();

        builder.HasIndex(t => t.Key).IsUnique();
    }
}
