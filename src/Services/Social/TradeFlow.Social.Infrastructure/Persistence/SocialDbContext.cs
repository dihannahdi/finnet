using Microsoft.EntityFrameworkCore;
using TradeFlow.Social.Domain.Entities;

namespace TradeFlow.Social.Infrastructure.Persistence;

public class SocialDbContext : DbContext
{
    public DbSet<TradeIdea> TradeIdeas => Set<TradeIdea>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<Like> Likes => Set<Like>();
    public DbSet<Follow> Follows => Set<Follow>();
    public DbSet<Notification> Notifications => Set<Notification>();

    public SocialDbContext(DbContextOptions<SocialDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TradeIdea>(entity =>
        {
            entity.ToTable("TradeIdeas");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Symbol).HasMaxLength(20);
            entity.Property(e => e.Direction).HasMaxLength(20);
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.Content).HasMaxLength(5000);
            entity.Property(e => e.AuthorName).HasMaxLength(100);
            entity.Property(e => e.TargetPrice).HasPrecision(18, 8);
            entity.Property(e => e.StopLoss).HasPrecision(18, 8);
            entity.HasIndex(e => e.AuthorId);
            entity.HasIndex(e => e.Symbol);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasMany(e => e.Comments).WithOne().HasForeignKey(c => c.TradeIdeaId);
            entity.HasMany(e => e.Likes).WithOne().HasForeignKey(l => l.TradeIdeaId);
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.ToTable("Comments");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Content).HasMaxLength(2000);
            entity.Property(e => e.UserName).HasMaxLength(100);
        });

        modelBuilder.Entity<Like>(entity =>
        {
            entity.ToTable("Likes");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.TradeIdeaId, e.UserId }).IsUnique();
        });

        modelBuilder.Entity<Follow>(entity =>
        {
            entity.ToTable("Follows");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.FollowerId, e.FollowingId }).IsUnique();
            entity.HasIndex(e => e.FollowerId);
            entity.HasIndex(e => e.FollowingId);
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.ToTable("Notifications");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Type).HasMaxLength(50);
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.Message).HasMaxLength(1000);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => new { e.UserId, e.IsRead });
        });
    }
}
