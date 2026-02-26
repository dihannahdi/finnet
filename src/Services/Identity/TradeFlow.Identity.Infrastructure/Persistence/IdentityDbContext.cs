using MediatR;
using Microsoft.EntityFrameworkCore;
using TradeFlow.Identity.Domain.Entities;

namespace TradeFlow.Identity.Infrastructure.Persistence;

public class IdentityDbContext : DbContext
{
    private readonly IMediator? _mediator;

    public DbSet<User> Users => Set<User>();

    public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options) { }

    public IdentityDbContext(DbContextOptions<IdentityDbContext> options, IMediator mediator) : base(options)
    {
        _mediator = mediator;
    }

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
            entity.Ignore(e => e.DomainEvents);
        });

        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Dispatch domain events before saving
        if (_mediator is not null)
        {
            var entitiesWithEvents = ChangeTracker.Entries<User>()
                .Where(e => e.Entity.DomainEvents.Any())
                .Select(e => e.Entity)
                .ToList();

            var domainEvents = entitiesWithEvents
                .SelectMany(e => e.DomainEvents)
                .ToList();

            entitiesWithEvents.ForEach(e => e.ClearDomainEvents());

            foreach (var domainEvent in domainEvents)
            {
                await _mediator.Publish(domainEvent, cancellationToken);
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
