namespace TradeFlow.Identity.Domain.Entities;

public class User
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Email { get; private set; } = default!;
    public string DisplayName { get; private set; } = default!;
    public string? PasswordHash { get; private set; }
    public string? GoogleId { get; private set; }
    public string? AvatarUrl { get; private set; }
    public string Role { get; private set; } = "Trader";
    public bool IsActive { get; private set; } = true;
    public string? RefreshToken { get; private set; }
    public DateTime? RefreshTokenExpiryTime { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; private set; }
    public DateTime? LastLoginAt { get; private set; }

    private User() { } // EF Core

    public static User Create(string email, string displayName, string passwordHash, string role = "Trader")
    {
        return new User
        {
            Email = email.ToLowerInvariant(),
            DisplayName = displayName,
            PasswordHash = passwordHash,
            Role = role
        };
    }

    public static User CreateFromGoogle(string email, string displayName, string googleId, string? avatarUrl)
    {
        return new User
        {
            Email = email.ToLowerInvariant(),
            DisplayName = displayName,
            GoogleId = googleId,
            AvatarUrl = avatarUrl,
            Role = "Trader"
        };
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
        Role = role;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool HasValidRefreshToken() =>
        RefreshToken != null && RefreshTokenExpiryTime > DateTime.UtcNow;
}
