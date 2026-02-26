using FluentAssertions;
using TradeFlow.Common.Domain;
using TradeFlow.Identity.Domain.Entities;
using Xunit;

namespace TradeFlow.Identity.Tests;

public class UserDomainTests
{
    [Fact]
    public void Create_ShouldInitializeCorrectly()
    {
        var user = User.Create("test@test.com", "Test User", "hash");

        user.Email.Should().Be("test@test.com");
        user.DisplayName.Should().Be("Test User");
        user.Role.Should().Be("Trader");
        user.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Create_ShouldNormalizeEmailToLowerCase()
    {
        var user = User.Create("Test@EXAMPLE.Com", "Test", "hash");
        user.Email.Should().Be("test@example.com");
    }

    [Fact]
    public void Create_ShouldRaiseDomainEvent()
    {
        var user = User.Create("test@test.com", "Test User", "hash");

        user.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<UserCreatedEvent>()
            .Which.Email.Should().Be("test@test.com");
    }

    [Fact]
    public void Create_WithCustomRole_ShouldSetRole()
    {
        var user = User.Create("test@test.com", "Admin User", "hash", "Admin");
        user.Role.Should().Be("Admin");
    }

    [Fact]
    public void Create_WithInvalidRole_ShouldThrow()
    {
        var act = () => User.Create("test@test.com", "Test", "hash", "SuperAdmin");
        act.Should().Throw<ArgumentException>().WithMessage("*Invalid role*");
    }

    [Fact]
    public void CreateFromGoogle_ShouldSetGoogleId()
    {
        var user = User.CreateFromGoogle("test@test.com", "Test User", "google-123", "http://photo.url");

        user.GoogleId.Should().Be("google-123");
        user.AvatarUrl.Should().Be("http://photo.url");
        user.PasswordHash.Should().BeNull();
    }

    [Fact]
    public void CreateFromGoogle_ShouldRaiseDomainEvent()
    {
        var user = User.CreateFromGoogle("test@test.com", "Test", "g-123", null);

        user.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<UserCreatedEvent>();
    }

    [Fact]
    public void UpdateRefreshToken_ShouldSetToken()
    {
        var user = User.Create("test@test.com", "Test User", "hash");
        var expiry = DateTime.UtcNow.AddDays(7);
        user.UpdateRefreshToken("new-token", expiry);

        user.RefreshToken.Should().Be("new-token");
        user.RefreshTokenExpiryTime.Should().Be(expiry);
        user.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void RevokeRefreshToken_ShouldClearToken()
    {
        var user = User.Create("test@test.com", "Test User", "hash");
        user.UpdateRefreshToken("token", DateTime.UtcNow.AddDays(7));
        user.RevokeRefreshToken();

        user.RefreshToken.Should().BeNull();
        user.RefreshTokenExpiryTime.Should().BeNull();
    }

    [Fact]
    public void HasValidRefreshToken_ShouldReturnTrue_WhenNotExpired()
    {
        var user = User.Create("test@test.com", "Test", "hash");
        user.UpdateRefreshToken("token", DateTime.UtcNow.AddDays(7));

        user.HasValidRefreshToken().Should().BeTrue();
    }

    [Fact]
    public void HasValidRefreshToken_ShouldReturnFalse_WhenExpired()
    {
        var user = User.Create("test@test.com", "Test", "hash");
        user.UpdateRefreshToken("token", DateTime.UtcNow.AddSeconds(-1));

        user.HasValidRefreshToken().Should().BeFalse();
    }

    [Fact]
    public void HasValidRefreshToken_ShouldReturnFalse_WhenNoToken()
    {
        var user = User.Create("test@test.com", "Test", "hash");
        user.HasValidRefreshToken().Should().BeFalse();
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveFalse()
    {
        var user = User.Create("test@test.com", "Test User", "hash");
        user.ClearDomainEvents(); // clear the creation event
        user.Deactivate();

        user.IsActive.Should().BeFalse();
        user.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<UserDeactivatedEvent>();
    }

    [Fact]
    public void Activate_ShouldSetIsActiveTrue()
    {
        var user = User.Create("test@test.com", "Test User", "hash");
        user.Deactivate();
        user.Activate();

        user.IsActive.Should().BeTrue();
    }

    [Fact]
    public void ChangeRole_WithValidRole_ShouldUpdate()
    {
        var user = User.Create("test@test.com", "Test", "hash");
        user.ChangeRole("Admin");

        user.Role.Should().Be("Admin");
        user.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void ChangeRole_WithInvalidRole_ShouldThrow()
    {
        var user = User.Create("test@test.com", "Test", "hash");
        var act = () => user.ChangeRole("InvalidRole");

        act.Should().Throw<ArgumentException>().WithMessage("*Invalid role*");
    }

    [Fact]
    public void UpdateProfile_ShouldUpdateDisplayNameAndAvatar()
    {
        var user = User.Create("test@test.com", "Old Name", "hash");
        user.UpdateProfile("New Name", "http://avatar.url");

        user.DisplayName.Should().Be("New Name");
        user.AvatarUrl.Should().Be("http://avatar.url");
    }

    [Fact]
    public void UpdateLastLogin_ShouldSetTimestamp()
    {
        var user = User.Create("test@test.com", "Test", "hash");
        user.UpdateLastLogin();

        user.LastLoginAt.Should().NotBeNull();
        user.LastLoginAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
    }
}
