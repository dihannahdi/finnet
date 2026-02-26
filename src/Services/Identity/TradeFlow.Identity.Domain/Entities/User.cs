using TradeFlow.Common.Domain;

namespace TradeFlow.Identity.Domain.Entities;

public class User
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Email { get; private set; } = default!;
    public string DisplayName { get; private set; } = default!;
    public string? PasswordHash { get; private set; }
    public string? GoogleId { get; private set; }
    public string? AvatarUrl { get; private set; }
    public string Role { get; private set; } = Common.Domain.Role.Trader;
    public bool IsActive { get; private set; } = true;
    public string? RefreshToken { get; private set; }
    public DateTime? RefreshTokenExpiryTime { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; private set; }
    public DateTime? LastLoginAt { get; private set; }

    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    public void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
    public void ClearDomainEvents() => _domainEvents.Clear();

    private User() { } // EF Core

    public static User Create(string email, string displayName, string passwordHash, string role = "Trader")
    {
        var validRole = Common.Domain.Role.From(role);
        var user = new User
        {
            Email = email.ToLowerInvariant(),
            DisplayName = displayName,
            PasswordHash = passwordHash,
            Role = validRole
        };
        user.AddDomainEvent(new UserCreatedEvent(user.Id, user.Email, user.DisplayName));
        return user;
    }

    public static User CreateFromGoogle(string email, string displayName, string googleId, string? avatarUrl)
    {
        var user = new User
        {
            Email = email.ToLowerInvariant(),
            DisplayName = displayName,
            GoogleId = googleId,
            AvatarUrl = avatarUrl,
            Role = Common.Domain.Role.Trader
        };
        user.AddDomainEvent(new UserCreatedEvent(user.Id, user.Email, user.DisplayName));
        return user;
    }

    public void UpdateRefreshToken(string refreshToken, DateTime expiryTime)
    {
        RefreshToken = refreshToken;
        RefreshTokenExpiryTime = expiryTime;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RevokeRefreshToken()
    {
        RefreshToken = null;
        RefreshTokenExpiryTime = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateLastLogin()
    {
        LastLoginAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateProfile(string displayName, string? avatarUrl)
    {
        DisplayName = displayName;
        AvatarUrl = avatarUrl;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangeRole(string role)
    {
        var validRole = Common.Domain.Role.From(role);
        Role = validRole;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new UserDeactivatedEvent(Id, Email));
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool HasValidRefreshToken() =>
        RefreshToken != null && RefreshTokenExpiryTime > DateTime.UtcNow;
}

// Domain Events
public sealed record UserCreatedEvent(Guid UserId, string Email, string DisplayName) : DomainEvent;
public sealed record UserDeactivatedEvent(Guid UserId, string Email) : DomainEvent;
