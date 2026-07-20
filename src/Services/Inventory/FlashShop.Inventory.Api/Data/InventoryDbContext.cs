using FlashShop.Inventory.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlashShop.Inventory.Api.Data;

public class InventoryDbContext : DbContext
{
    public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options) { }

    public DbSet<Stock> Stocks => Set<Stock>();
    public DbSet<StockReservation> StockReservations => Set<StockReservation>();
    public DbSet<StockHistory> StockHistory => Set<StockHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(InventoryDbContext).Assembly);
    }
}
