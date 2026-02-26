using Microsoft.EntityFrameworkCore;
using TradeFlow.Portfolio.Domain.Entities;

namespace TradeFlow.Portfolio.Infrastructure.Persistence;

public class PortfolioDbContext : DbContext
{
    public DbSet<UserPortfolio> Portfolios => Set<UserPortfolio>();
    public DbSet<Position> Positions => Set<Position>();
    public DbSet<Trade> Trades => Set<Trade>();

    public PortfolioDbContext(DbContextOptions<PortfolioDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserPortfolio>(entity =>
        {
            entity.ToTable("Portfolios");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId).IsUnique();
            entity.Property(e => e.CashBalance).HasPrecision(18, 2);
            entity.HasMany(e => e.Positions).WithOne().HasForeignKey(p => p.PortfolioId);
            entity.HasMany(e => e.Trades).WithOne().HasForeignKey(t => t.PortfolioId);
        });

        modelBuilder.Entity<Position>(entity =>
        {
            entity.ToTable("Positions");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Symbol).HasMaxLength(20);
            entity.Property(e => e.Quantity).HasPrecision(18, 8);
            entity.Property(e => e.AveragePrice).HasPrecision(18, 8);
            entity.Property(e => e.TotalCost).HasPrecision(18, 2);
        });

        modelBuilder.Entity<Trade>(entity =>
        {
            entity.ToTable("Trades");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Symbol).HasMaxLength(20);
            entity.Property(e => e.Side).HasMaxLength(10);
            entity.Property(e => e.Quantity).HasPrecision(18, 8);
            entity.Property(e => e.Price).HasPrecision(18, 8);
            entity.Property(e => e.TotalValue).HasPrecision(18, 2);
            entity.HasIndex(e => e.ExecutedAt);
        });
    }
}
