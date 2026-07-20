using FlashShop.Inventory.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlashShop.Inventory.Api.Data.Configurations;

public class StockConfiguration : IEntityTypeConfiguration<Stock>
{
    public void Configure(EntityTypeBuilder<Stock> builder)
    {
        builder.HasKey(s => s.ProductId);

        builder.Property(s => s.RowVersion)
            .IsRowVersion();

        builder.Property(s => s.AvailableQuantity)
            .HasDefaultValue(0);

        builder.Property(s => s.ReservedQuantity)
            .HasDefaultValue(0);

        builder.Property(s => s.MinThreshold)
            .HasDefaultValue(10);

        builder.Property(s => s.UpdatedAt)
            .HasDefaultValueSql("GETUTCDATE()");
    }
}
