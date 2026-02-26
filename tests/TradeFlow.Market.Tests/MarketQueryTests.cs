using FluentAssertions;
using Moq;
using TradeFlow.Market.Application.Queries;
using TradeFlow.Market.Domain.Entities;
using TradeFlow.Market.Domain.Interfaces;
using Xunit;

namespace TradeFlow.Market.Tests;

public class MarketQueryTests
{
    [Fact]
    public async Task GetMarketPrice_ShouldReturnCachedPrice_WhenAvailable()
    {
        // Arrange
        var cache = new Mock<IMarketDataCache>();
        var repo = new Mock<IMarketPriceRepository>();
        cache.Setup(x => x.GetPriceAsync("AAPL"))
            .ReturnsAsync(MarketPrice.Create("AAPL", "Apple Inc.", "Stock", 150m));

        var handler = new GetMarketPriceQueryHandler(cache.Object, repo.Object);

        // Act
        var result = await handler.Handle(new GetMarketPriceQuery { Symbol = "AAPL" }, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        repo.Verify(x => x.GetBySymbolAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetMarketPrice_ShouldFallbackToDb_WhenCacheMiss()
    {
        // Arrange
        var cache = new Mock<IMarketDataCache>();
        var repo = new Mock<IMarketPriceRepository>();
        cache.Setup(x => x.GetPriceAsync("AAPL"))
            .ReturnsAsync((MarketPrice?)null);
        repo.Setup(x => x.GetBySymbolAsync("AAPL", It.IsAny<CancellationToken>()))
            .ReturnsAsync(MarketPrice.Create("AAPL", "Apple Inc.", "Stock", 150m));

        var handler = new GetMarketPriceQueryHandler(cache.Object, repo.Object);

        // Act
        var result = await handler.Handle(new GetMarketPriceQuery { Symbol = "AAPL" }, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        repo.Verify(x => x.GetBySymbolAsync("AAPL", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetMarketPrice_ShouldReturnNotFound_WhenNoData()
    {
        // Arrange
        var cache = new Mock<IMarketDataCache>();
        var repo = new Mock<IMarketPriceRepository>();
        cache.Setup(x => x.GetPriceAsync("XYZ"))
            .ReturnsAsync((MarketPrice?)null);
        repo.Setup(x => x.GetBySymbolAsync("XYZ", It.IsAny<CancellationToken>()))
            .ReturnsAsync((MarketPrice?)null);

        var handler = new GetMarketPriceQueryHandler(cache.Object, repo.Object);

        // Act
        var result = await handler.Handle(new GetMarketPriceQuery { Symbol = "XYZ" }, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not found");
    }
}
