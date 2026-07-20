using FlashShop.Inventory.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlashShop.Inventory.Api.Data.Configurations;

public class StockReservationConfiguration : IEntityTypeConfiguration<StockReservation>
{
    public void Configure(EntityTypeBuilder<StockReservation> builder)
    {
        builder.HasKey(r => r.Id);

        builder.HasIndex(r => new { r.Status, r.ExpiresAt })
            .HasFilter("Status = 'Reserved'");

        builder.HasIndex(r => r.OrderId);

        builder.HasIndex(r => r.ProductId);

        builder.Property(r => r.Status)
            .HasMaxLength(20)
            .HasDefaultValue("Reserved");

        builder.Property(r => r.ExpiresAt)
            .IsRequired();

        builder.Property(r => r.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");
    }
}
