using FluentAssertions;
using TradeFlow.Common.Domain;
using Xunit;

namespace TradeFlow.Identity.Tests;

public class RoleValueObjectTests
{
    [Fact]
    public void From_ValidRole_ShouldReturnRole()
    {
        var role = Role.From("Trader");
        role.Value.Should().Be("Trader");
    }

    [Fact]
    public void From_CaseInsensitive_ShouldReturnRole()
    {
        var role = Role.From("admin");
        role.Value.Should().Be("Admin");
    }

    [Fact]
    public void From_InvalidRole_ShouldThrow()
    {
        var act = () => Role.From("SuperAdmin");
        act.Should().Throw<ArgumentException>().WithMessage("*Invalid role*");
    }

    [Fact]
    public void From_EmptyString_ShouldThrow()
    {
        var act = () => Role.From("");
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void TryFrom_ValidRole_ShouldReturnTrue()
    {
        Role.TryFrom("Trader", out var role).Should().BeTrue();
        role!.Value.Should().Be("Trader");
    }

    [Fact]
    public void TryFrom_InvalidRole_ShouldReturnFalse()
    {
        Role.TryFrom("Invalid", out var role).Should().BeFalse();
        role.Should().BeNull();
    }

    [Fact]
    public void Equality_SameRole_ShouldBeEqual()
    {
        var role1 = Role.From("Trader");
        var role2 = Role.From("Trader");

        role1.Should().Be(role2);
        (role1 == role2).Should().BeTrue();
    }

    [Fact]
    public void Equality_DifferentRole_ShouldNotBeEqual()
    {
        var role1 = Role.From("Trader");
        var role2 = Role.From("Admin");

        role1.Should().NotBe(role2);
        (role1 != role2).Should().BeTrue();
    }

    [Fact]
    public void ImplicitConversion_ShouldReturnString()
    {
        string roleString = Role.Trader;
        roleString.Should().Be("Trader");
    }

    [Fact]
    public void All_ShouldReturnAllValidRoles()
    {
        Role.All.Should().HaveCount(3);
        Role.All.Should().Contain(Role.Trader);
        Role.All.Should().Contain(Role.Admin);
        Role.All.Should().Contain(Role.Analyst);
    }

    [Fact]
    public void StaticMembers_ShouldMatchExpectedValues()
    {
        Role.Trader.Value.Should().Be("Trader");
        Role.Admin.Value.Should().Be("Admin");
        Role.Analyst.Value.Should().Be("Analyst");
    }

    [Fact]
    public void ToString_ShouldReturnValue()
    {
        Role.Trader.ToString().Should().Be("Trader");
    }
}

public class DateTimeProviderTests
{
    [Fact]
    public void SystemDateTimeProvider_ShouldReturnCurrentUtcTime()
    {
        var provider = new SystemDateTimeProvider();
        var before = DateTime.UtcNow;
        var result = provider.UtcNow;
        var after = DateTime.UtcNow;

        result.Should().BeOnOrAfter(before);
        result.Should().BeOnOrBefore(after);
    }

    [Fact]
    public void FakeDateTimeProvider_ShouldReturnFixedTime()
    {
        var fixedTime = new DateTime(2024, 6, 15, 12, 0, 0, DateTimeKind.Utc);
        var provider = new FakeDateTimeProvider(fixedTime);

        provider.UtcNow.Should().Be(fixedTime);
    }
}

/// <summary>
/// Test double for IDateTimeProvider — enables deterministic time in tests.
/// </summary>
public class FakeDateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow { get; set; }

    public FakeDateTimeProvider(DateTime fixedTime)
    {
        UtcNow = fixedTime;
    }
}
