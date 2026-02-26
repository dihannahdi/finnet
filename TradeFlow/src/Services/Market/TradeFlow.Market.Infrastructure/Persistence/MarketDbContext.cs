using Microsoft.EntityFrameworkCore;
using TradeFlow.Market.Domain.Entities;

namespace TradeFlow.Market.Infrastructure.Persistence;

public class MarketDbContext : DbContext
{
    public DbSet<MarketPrice> MarketPrices => Set<MarketPrice>();
    public DbSet<Watchlist> Watchlists => Set<Watchlist>();
    public DbSet<WatchlistItem> WatchlistItems => Set<WatchlistItem>();

    public MarketDbContext(DbContextOptions<MarketDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MarketPrice>(entity =>
        {
            entity.ToTable("MarketPrices");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Symbol).HasMaxLength(20).IsRequired();
            entity.HasIndex(e => e.Symbol).IsUnique();
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.AssetType).HasMaxLength(20);
            entity.Property(e => e.CurrentPrice).HasPrecision(18, 8);
            entity.Property(e => e.PreviousClose).HasPrecision(18, 8);
            entity.Property(e => e.High).HasPrecision(18, 8);
            entity.Property(e => e.Low).HasPrecision(18, 8);
            entity.Property(e => e.Open).HasPrecision(18, 8);
            entity.Property(e => e.Volume).HasPrecision(18, 2);
            entity.Property(e => e.Change).HasPrecision(18, 8);
            entity.Property(e => e.ChangePercent).HasPrecision(18, 4);
        });

        modelBuilder.Entity<Watchlist>(entity =>
        {
            entity.ToTable("Watchlists");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.HasMany(e => e.Items).WithOne().HasForeignKey(e => e.WatchlistId);
        });

        modelBuilder.Entity<WatchlistItem>(entity =>
        {
            entity.ToTable("WatchlistItems");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Symbol).HasMaxLength(20);
        });
    }
}
