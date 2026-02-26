using FluentAssertions;
using TradeFlow.Portfolio.Domain.Entities;
using Xunit;

namespace TradeFlow.Portfolio.Tests;

public class PortfolioAggregateTests
{
    [Fact]
    public void Create_ShouldInitializeWith100kCash()
    {
        var portfolio = UserPortfolio.Create(Guid.NewGuid());

        portfolio.CashBalance.Should().Be(100_000m);
        portfolio.Positions.Should().BeEmpty();
        portfolio.Trades.Should().BeEmpty();
    }

    [Fact]
    public void ExecuteBuy_ShouldCreatePosition_WhenSufficientCash()
    {
        var portfolio = UserPortfolio.Create(Guid.NewGuid());
        portfolio.ExecuteBuy("AAPL", 10m, 150.00m);

        portfolio.Positions.Should().HaveCount(1);
        portfolio.Positions.First().Symbol.Should().Be("AAPL");
        portfolio.Positions.First().Quantity.Should().Be(10m);
        portfolio.Positions.First().AveragePrice.Should().Be(150.00m);
        portfolio.CashBalance.Should().Be(98_500m);
        portfolio.Trades.Should().HaveCount(1);
    }

    [Fact]
    public void ExecuteBuy_ShouldThrow_WhenInsufficientCash()
    {
        var portfolio = UserPortfolio.Create(Guid.NewGuid());

        var act = () => portfolio.ExecuteBuy("AAPL", 10000m, 150.00m);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Insufficient*");
    }

    [Fact]
    public void ExecuteBuy_ShouldAddToExistingPosition()
    {
        var portfolio = UserPortfolio.Create(Guid.NewGuid());
        portfolio.ExecuteBuy("AAPL", 10m, 150.00m);
        portfolio.ExecuteBuy("AAPL", 10m, 160.00m);

        portfolio.Positions.Should().HaveCount(1);
        portfolio.Positions.First().Quantity.Should().Be(20m);
        portfolio.Positions.First().AveragePrice.Should().Be(155.00m);
    }

    [Fact]
    public void ExecuteSell_ShouldReducePosition()
    {
        var portfolio = UserPortfolio.Create(Guid.NewGuid());
        portfolio.ExecuteBuy("AAPL", 10m, 150.00m);
        portfolio.ExecuteSell("AAPL", 5m, 160.00m);

        portfolio.Positions.First().Quantity.Should().Be(5m);
        portfolio.CashBalance.Should().Be(99_300m); // 100k - 1500 + 800
        portfolio.Trades.Should().HaveCount(2);
    }

    [Fact]
    public void ExecuteSell_ShouldThrow_WhenNoPosition()
    {
        var portfolio = UserPortfolio.Create(Guid.NewGuid());

        var act = () => portfolio.ExecuteSell("AAPL", 5m, 150.00m);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Insufficient*");
    }

    [Fact]
    public void ExecuteSell_ShouldThrow_WhenInsufficientShares()
    {
        var portfolio = UserPortfolio.Create(Guid.NewGuid());
        portfolio.ExecuteBuy("AAPL", 5m, 150.00m);

        var act = () => portfolio.ExecuteSell("AAPL", 10m, 160.00m);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Insufficient*");
    }

    [Fact]
    public void GetTotalValue_ShouldCalculateCorrectly()
    {
        var portfolio = UserPortfolio.Create(Guid.NewGuid());
        portfolio.ExecuteBuy("AAPL", 10m, 150.00m);

        // Total value = cash + position value at current price
        // CashBalance = 100000 - 1500 = 98500
        // Position value at $160 = 10 * 160 = 1600
        // Total = 98500 + 1600 = 100100
        var totalValue = portfolio.GetTotalValue(symbol => 160m);
        totalValue.Should().Be(100_100m);
    }
}
