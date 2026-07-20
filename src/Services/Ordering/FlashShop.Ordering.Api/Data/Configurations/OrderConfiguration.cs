using FlashShop.Ordering.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlashShop.Ordering.Api.Data.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);
        builder.Property(o => o.OrderNumber).HasMaxLength(20).IsRequired();
        builder.HasIndex(o => o.OrderNumber).IsUnique();
        builder.Property(o => o.UserEmail).HasMaxLength(256).IsRequired();
        builder.Property(o => o.Status).HasMaxLength(20).HasDefaultValue("Pending");
        builder.Property(o => o.TotalAmount).HasPrecision(18, 2);
        builder.Property(o => o.ShippingAddress).HasMaxLength(500).IsRequired();
        builder.Property(o => o.RecipientName).HasMaxLength(200).IsRequired();
        builder.Property(o => o.RecipientPhone).HasMaxLength(20).IsRequired();
        builder.Property(o => o.PaymentMethod).HasMaxLength(30).HasDefaultValue("Wallet");
        builder.Property(o => o.Notes).HasMaxLength(500);
        builder.Property(o => o.CancelReason).HasMaxLength(500);
        builder.Property(o => o.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        builder.HasIndex(o => o.UserId);
        builder.HasIndex(o => new { o.Status, o.PaymentDeadline })
            .HasFilter("[Status] = 'AwaitingPayment'");
        builder.HasMany(o => o.Items).WithOne(i => i.Order)
            .HasForeignKey(i => i.OrderId).OnDelete(DeleteBehavior.Cascade);
    }
}
