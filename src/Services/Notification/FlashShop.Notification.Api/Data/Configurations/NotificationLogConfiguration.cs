using FlashShop.Notification.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlashShop.Notification.Api.Data.Configurations;

public class NotificationLogConfiguration : IEntityTypeConfiguration<NotificationLog>
{
    public void Configure(EntityTypeBuilder<NotificationLog> builder)
    {
        builder.HasKey(n => n.Id);
        builder.Property(n => n.RecipientEmail).IsRequired().HasMaxLength(256);
        builder.Property(n => n.Subject).IsRequired().HasMaxLength(500);
        builder.Property(n => n.Body).IsRequired();
        builder.Property(n => n.TemplateKey).IsRequired().HasMaxLength(100);
        builder.Property(n => n.ErrorMessage).HasMaxLength(1000);

        builder.HasIndex(n => n.CreatedAt);
        builder.HasIndex(n => n.TemplateKey);
    }
}
