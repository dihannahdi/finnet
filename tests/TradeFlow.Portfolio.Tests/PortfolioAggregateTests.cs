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
    public void Create_ShouldSetUserId()
    {
        var userId = Guid.NewGuid();
        var portfolio = UserPortfolio.Create(userId);

        portfolio.UserId.Should().Be(userId);
        portfolio.Id.Should().NotBeEmpty();
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
    public void ExecuteBuy_ShouldThrow_WhenExactlyOverBalance()
    {
        var portfolio = UserPortfolio.Create(Guid.NewGuid());
        var act = () => portfolio.ExecuteBuy("AAPL", 1m, 100_001m);
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void ExecuteBuy_ShouldSucceed_WhenExactBalance()
    {
        var portfolio = UserPortfolio.Create(Guid.NewGuid());
        var trade = portfolio.ExecuteBuy("AAPL", 1000m, 100m);

        portfolio.CashBalance.Should().Be(0m);
        trade.Should().NotBeNull();
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
    public void ExecuteBuy_MultipleSymbols_ShouldCreateSeparatePositions()
    {
        var portfolio = UserPortfolio.Create(Guid.NewGuid());
        portfolio.ExecuteBuy("AAPL", 10m, 150m);
        portfolio.ExecuteBuy("GOOGL", 5m, 200m);

        portfolio.Positions.Should().HaveCount(2);
        portfolio.Trades.Should().HaveCount(2);
    }

    [Fact]
    public void ExecuteBuy_ShouldNormalizeSymbolToUpperCase()
    {
        var portfolio = UserPortfolio.Create(Guid.NewGuid());
        portfolio.ExecuteBuy("aapl", 10m, 150m);

        portfolio.Positions.First().Symbol.Should().Be("AAPL");
        portfolio.Trades.First().Symbol.Should().Be("AAPL");
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
    public void ExecuteSell_AllShares_ShouldRemovePosition()
    {
        var portfolio = UserPortfolio.Create(Guid.NewGuid());
        portfolio.ExecuteBuy("AAPL", 10m, 150m);
        portfolio.ExecuteSell("AAPL", 10m, 160m);

        portfolio.Positions.Should().BeEmpty();
        portfolio.CashBalance.Should().Be(100_100m);
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

        var totalValue = portfolio.GetTotalValue(symbol => 160m);
        totalValue.Should().Be(100_100m);
    }

    [Fact]
    public void GetTotalValue_WithMultiplePositions_ShouldSumAll()
    {
        var portfolio = UserPortfolio.Create(Guid.NewGuid());
        portfolio.ExecuteBuy("AAPL", 10m, 150m);
        portfolio.ExecuteBuy("GOOGL", 5m, 200m);

        var totalValue = portfolio.GetTotalValue(sym => sym == "AAPL" ? 160m : 210m);
        totalValue.Should().Be(100_150m);
    }

    [Fact]
    public void GetTotalPnL_ShouldCalculateProfit()
    {
        var portfolio = UserPortfolio.Create(Guid.NewGuid());
        portfolio.ExecuteBuy("AAPL", 10m, 150m);

        var pnl = portfolio.GetTotalPnL(_ => 160m);
        pnl.Should().Be(100m);
    }

    [Fact]
    public void GetTotalPnL_ShouldCalculateLoss()
    {
        var portfolio = UserPortfolio.Create(Guid.NewGuid());
        portfolio.ExecuteBuy("AAPL", 10m, 150m);

        var pnl = portfolio.GetTotalPnL(_ => 140m);
        pnl.Should().Be(-100m);
    }

    [Fact]
    public void Trade_Create_ShouldSetCorrectTotalValue()
    {
        var portfolio = UserPortfolio.Create(Guid.NewGuid());
        var trade = portfolio.ExecuteBuy("AAPL", 10m, 150m);

        trade.TotalValue.Should().Be(1500m);
        trade.Side.Should().Be("Buy");
    }

    [Fact]
    public void Position_AveragePrice_ShouldCalculateWeightedAverage()
    {
        var portfolio = UserPortfolio.Create(Guid.NewGuid());
        portfolio.ExecuteBuy("AAPL", 10m, 100m);
        portfolio.ExecuteBuy("AAPL", 30m, 200m);

        var position = portfolio.Positions.First();
        position.AveragePrice.Should().Be(175m);
        position.Quantity.Should().Be(40m);
    }
}
