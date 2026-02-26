using FluentAssertions;
using MassTransit;
using Moq;
using TradeFlow.Portfolio.Application.Commands;
using TradeFlow.Portfolio.Domain.Entities;
using TradeFlow.Portfolio.Domain.Interfaces;
using Xunit;

namespace TradeFlow.Portfolio.Tests;

public class TradeCommandTests
{
    private readonly Mock<IPortfolioRepository> _portfolioRepo = new();
    private readonly Mock<IPublishEndpoint> _publishEndpoint = new();

    [Fact]
    public async Task ExecuteTrade_Buy_ShouldCreatePortfolio_WhenNotExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _portfolioRepo.Setup(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserPortfolio?)null);

        var handler = new ExecuteTradeCommandHandler(_portfolioRepo.Object, _publishEndpoint.Object);
        var command = new ExecuteTradeCommand { UserId = userId, Symbol = "AAPL", Side = "Buy", Quantity = 10, Price = 150m };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _portfolioRepo.Verify(x => x.AddAsync(It.IsAny<UserPortfolio>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteTrade_Buy_ShouldUpdateExistingPortfolio()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var portfolio = UserPortfolio.Create(userId);
        _portfolioRepo.Setup(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(portfolio);

        var handler = new ExecuteTradeCommandHandler(_portfolioRepo.Object, _publishEndpoint.Object);
        var command = new ExecuteTradeCommand { UserId = userId, Symbol = "AAPL", Side = "Buy", Quantity = 10, Price = 150m };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _portfolioRepo.Verify(x => x.UpdateAsync(It.IsAny<UserPortfolio>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
