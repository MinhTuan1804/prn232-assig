using FlashShop.Identity.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlashShop.Identity.Api.Data.Configurations;

public class WalletConfiguration : IEntityTypeConfiguration<Wallet>
{
    public void Configure(EntityTypeBuilder<Wallet> builder)
    {
        builder.HasKey(w => w.Id);
        builder.Property(w => w.Balance).HasPrecision(18, 2).HasDefaultValue(0m);
        builder.Property(w => w.Currency).HasMaxLength(3).HasDefaultValue("VND");
        builder.Property(w => w.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
        builder.HasIndex(w => w.UserId).IsUnique();
        builder.HasOne(w => w.User).WithOne().HasForeignKey<Wallet>(w => w.UserId);
        builder.HasMany(w => w.Transactions).WithOne(t => t.Wallet).HasForeignKey(t => t.WalletId);
    }
}
