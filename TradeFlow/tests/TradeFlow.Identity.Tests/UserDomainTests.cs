using FluentAssertions;
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
    public void CreateFromGoogle_ShouldSetGoogleId()
    {
        var user = User.CreateFromGoogle("test@test.com", "Test User", "google-123", "http://photo.url");

        user.GoogleId.Should().Be("google-123");
        user.AvatarUrl.Should().Be("http://photo.url");
    }

    [Fact]
    public void UpdateRefreshToken_ShouldSetToken()
    {
        var user = User.Create("test@test.com", "Test User", "hash");
        user.UpdateRefreshToken("new-token", DateTime.UtcNow.AddDays(7));

        user.RefreshToken.Should().Be("new-token");
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveFalse()
    {
        var user = User.Create("test@test.com", "Test User", "hash");
        user.Deactivate();

        user.IsActive.Should().BeFalse();
    }
}
