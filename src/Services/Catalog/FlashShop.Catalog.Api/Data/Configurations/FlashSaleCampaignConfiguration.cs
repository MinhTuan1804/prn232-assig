using FlashShop.Catalog.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlashShop.Catalog.Api.Data.Configurations;

public class FlashSaleCampaignConfiguration : IEntityTypeConfiguration<FlashSaleCampaign>
{
    public void Configure(EntityTypeBuilder<FlashSaleCampaign> builder)
    {
        builder.HasKey(f => f.Id);

        builder.Property(f => f.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(f => f.Status)
            .IsRequired()
            .HasMaxLength(20)
            .HasDefaultValue("Scheduled");

        builder.HasIndex(f => f.StartTime);
    }
}
