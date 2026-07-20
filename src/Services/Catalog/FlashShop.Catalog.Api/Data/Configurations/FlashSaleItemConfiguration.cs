using FlashShop.Catalog.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlashShop.Catalog.Api.Data.Configurations;

public class FlashSaleItemConfiguration : IEntityTypeConfiguration<FlashSaleItem>
{
    public void Configure(EntityTypeBuilder<FlashSaleItem> builder)
    {
        builder.HasKey(fi => fi.Id);

        builder.HasIndex(fi => new { fi.CampaignId, fi.ProductId }).IsUnique();

        builder.Property(fi => fi.FlashSalePrice)
            .HasPrecision(18, 2);

        builder.Property(fi => fi.SoldQuantity)
            .HasDefaultValue(0);

        builder.Property(fi => fi.MaxPerUser)
            .HasDefaultValue(1);

        builder.HasOne(fi => fi.Campaign)
            .WithMany(c => c.Items)
            .HasForeignKey(fi => fi.CampaignId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(fi => fi.Product)
            .WithMany()
            .HasForeignKey(fi => fi.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
