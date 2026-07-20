using FlashShop.Ordering.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlashShop.Ordering.Api.Data.Configurations;

public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.ProductName).HasMaxLength(300).IsRequired();
        builder.Property(c => c.UnitPrice).HasPrecision(18, 2);
        builder.Property(c => c.Quantity).HasDefaultValue(1);
        builder.Property(c => c.ImageUrl).HasMaxLength(500);
        builder.Property(c => c.AddedAt).HasDefaultValueSql("GETUTCDATE()");
        builder.HasIndex(c => c.UserId);
        builder.HasIndex(c => new { c.UserId, c.ProductId }).IsUnique();
    }
}
