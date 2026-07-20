using FlashShop.Inventory.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlashShop.Inventory.Api.Data.Configurations;

public class StockHistoryConfiguration : IEntityTypeConfiguration<StockHistory>
{
    public void Configure(EntityTypeBuilder<StockHistory> builder)
    {
        builder.HasKey(h => h.Id);

        builder.Property(h => h.Id)
            .ValueGeneratedOnAdd();

        builder.Property(h => h.ChangeType)
            .HasMaxLength(30);

        builder.Property(h => h.ReferenceId)
            .HasMaxLength(100);

        builder.HasIndex(h => h.ProductId);

        builder.Property(h => h.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");
    }
}
