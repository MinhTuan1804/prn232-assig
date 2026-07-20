using FlashShop.Catalog.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlashShop.Catalog.Api.Data;

public class CatalogDbContext : DbContext
{
    public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options) { }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductImage> ProductImages => Set<ProductImage>();
    public DbSet<FlashSaleCampaign> FlashSaleCampaigns => Set<FlashSaleCampaign>();
    public DbSet<FlashSaleItem> FlashSaleItems => Set<FlashSaleItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CatalogDbContext).Assembly);
    }
}
