using FluentAssertions;
using MassTransit;
using Moq;
using TradeFlow.MessageContracts.Events;
using TradeFlow.Portfolio.Application.Commands;
using TradeFlow.Portfolio.Domain.Entities;
using TradeFlow.Portfolio.Domain.Interfaces;
using Xunit;

namespace TradeFlow.Portfolio.Tests;

public class TradeCommandTests
{
    private readonly Mock<IPortfolioRepository> _portfolioRepo = new();
    private readonly Mock<IPublishEndpoint> _publishEndpoint = new();

    private ExecuteTradeCommandHandler CreateHandler() =>
        new(_portfolioRepo.Object, _publishEndpoint.Object);

    [Fact]
    public async Task ExecuteTrade_Buy_ShouldCreatePortfolio_WhenNotExists()
    {
        var userId = Guid.NewGuid();
        _portfolioRepo.Setup(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserPortfolio?)null);

        var result = await CreateHandler().Handle(
            new ExecuteTradeCommand { UserId = userId, Symbol = "AAPL", Side = "Buy", Quantity = 10, Price = 150m },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _portfolioRepo.Verify(x => x.AddAsync(It.IsAny<UserPortfolio>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteTrade_Buy_ShouldUpdateExistingPortfolio()
    {
        var userId = Guid.NewGuid();
        var portfolio = UserPortfolio.Create(userId);
        _portfolioRepo.Setup(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(portfolio);

        var result = await CreateHandler().Handle(
            new ExecuteTradeCommand { UserId = userId, Symbol = "AAPL", Side = "Buy", Quantity = 10, Price = 150m },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _portfolioRepo.Verify(x => x.UpdateAsync(It.IsAny<UserPortfolio>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteTrade_Sell_ShouldUpdateExistingPortfolio()
    {
        var userId = Guid.NewGuid();
        var portfolio = UserPortfolio.Create(userId);
        portfolio.ExecuteBuy("AAPL", 10m, 150m);
        _portfolioRepo.Setup(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(portfolio);

        var result = await CreateHandler().Handle(
            new ExecuteTradeCommand { UserId = userId, Symbol = "AAPL", Side = "Sell", Quantity = 5, Price = 160m },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        portfolio.CashBalance.Should().Be(99_300m);
    }

    [Fact]
    public async Task ExecuteTrade_Buy_ShouldPublishEvent()
    {
        var userId = Guid.NewGuid();
        _portfolioRepo.Setup(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserPortfolio?)null);

        await CreateHandler().Handle(
            new ExecuteTradeCommand { UserId = userId, Symbol = "AAPL", Side = "Buy", Quantity = 10, Price = 150m },
            CancellationToken.None);

        _publishEndpoint.Verify(x => x.Publish(It.IsAny<TradeExecuted>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteTrade_Buy_ShouldDeductCashFromPortfolio()
    {
        var userId = Guid.NewGuid();
        var portfolio = UserPortfolio.Create(userId);
        _portfolioRepo.Setup(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(portfolio);

        await CreateHandler().Handle(
            new ExecuteTradeCommand { UserId = userId, Symbol = "AAPL", Side = "Buy", Quantity = 10, Price = 150m },
            CancellationToken.None);

        portfolio.CashBalance.Should().Be(98_500m);
        portfolio.Positions.Should().HaveCount(1);
    }

    [Fact]
    public async Task ExecuteTrade_Sell_ShouldFail_WhenInsufficientShares()
    {
        var userId = Guid.NewGuid();
        var portfolio = UserPortfolio.Create(userId);
        _portfolioRepo.Setup(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(portfolio);

        var result = await CreateHandler().Handle(
            new ExecuteTradeCommand { UserId = userId, Symbol = "AAPL", Side = "Sell", Quantity = 5, Price = 150m },
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task ExecuteTrade_Buy_ShouldFail_WhenInsufficientCash()
    {
        var userId = Guid.NewGuid();
        var portfolio = UserPortfolio.Create(userId);
        _portfolioRepo.Setup(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(portfolio);

        var result = await CreateHandler().Handle(
            new ExecuteTradeCommand { UserId = userId, Symbol = "AAPL", Side = "Buy", Quantity = 10000, Price = 150m },
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }
}
