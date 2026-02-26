using Microsoft.EntityFrameworkCore;
using TradeFlow.Identity.Domain.Entities;

namespace TradeFlow.Identity.Infrastructure.Persistence;

public class IdentityDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();

    public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).HasMaxLength(256).IsRequired();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.DisplayName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.PasswordHash).HasMaxLength(512);
            entity.Property(e => e.GoogleId).HasMaxLength(256);
            entity.HasIndex(e => e.GoogleId).IsUnique().HasFilter("\"GoogleId\" IS NOT NULL");
            entity.Property(e => e.AvatarUrl).HasMaxLength(1024);
            entity.Property(e => e.Role).HasMaxLength(50).IsRequired();
            entity.Property(e => e.RefreshToken).HasMaxLength(512);
            entity.HasIndex(e => e.RefreshToken).IsUnique().HasFilter("\"RefreshToken\" IS NOT NULL");
        });

        base.OnModelCreating(modelBuilder);
    }
}
