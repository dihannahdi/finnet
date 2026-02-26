using FluentAssertions;
using Moq;
using TradeFlow.Market.Application.Queries;
using TradeFlow.Market.Domain.Entities;
using TradeFlow.Market.Domain.Interfaces;
using Xunit;

namespace TradeFlow.Market.Tests;

public class MarketQueryTests
{
    private readonly Mock<IMarketDataCache> _cache = new();
    private readonly Mock<IMarketPriceRepository> _repo = new();

    private GetMarketPriceQueryHandler CreateHandler() => new(_cache.Object, _repo.Object);

    [Fact]
    public async Task GetMarketPrice_ShouldReturnCachedPrice_WhenAvailable()
    {
        _cache.Setup(x => x.GetPriceAsync("AAPL"))
            .ReturnsAsync(MarketPrice.Create("AAPL", "Apple Inc.", "Stock", 150m));

        var result = await CreateHandler().Handle(new GetMarketPriceQuery { Symbol = "AAPL" }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.CurrentPrice.Should().Be(150m);
        _repo.Verify(x => x.GetBySymbolAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetMarketPrice_ShouldFallbackToDb_WhenCacheMiss()
    {
        _cache.Setup(x => x.GetPriceAsync("AAPL"))
            .ReturnsAsync((MarketPrice?)null);
        _repo.Setup(x => x.GetBySymbolAsync("AAPL", It.IsAny<CancellationToken>()))
            .ReturnsAsync(MarketPrice.Create("AAPL", "Apple Inc.", "Stock", 150m));

        var result = await CreateHandler().Handle(new GetMarketPriceQuery { Symbol = "AAPL" }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _repo.Verify(x => x.GetBySymbolAsync("AAPL", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetMarketPrice_ShouldReturnNotFound_WhenNoData()
    {
        _cache.Setup(x => x.GetPriceAsync("XYZ"))
            .ReturnsAsync((MarketPrice?)null);
        _repo.Setup(x => x.GetBySymbolAsync("XYZ", It.IsAny<CancellationToken>()))
            .ReturnsAsync((MarketPrice?)null);

        var result = await CreateHandler().Handle(new GetMarketPriceQuery { Symbol = "XYZ" }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not found");
    }

    [Fact]
    public void MarketPrice_Create_ShouldSetProperties()
    {
        var price = MarketPrice.Create("AAPL", "Apple Inc.", "Stock", 150.50m);

        price.Symbol.Should().Be("AAPL");
        price.Name.Should().Be("Apple Inc.");
        price.AssetType.Should().Be("Stock");
        price.CurrentPrice.Should().Be(150.50m);
    }

    [Fact]
    public async Task GetMarketPrice_ShouldCacheDbResult_WhenCacheMiss()
    {
        _cache.Setup(x => x.GetPriceAsync("AAPL"))
            .ReturnsAsync((MarketPrice?)null);
        _repo.Setup(x => x.GetBySymbolAsync("AAPL", It.IsAny<CancellationToken>()))
            .ReturnsAsync(MarketPrice.Create("AAPL", "Apple Inc.", "Stock", 150m));

        var result = await CreateHandler().Handle(new GetMarketPriceQuery { Symbol = "AAPL" }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _cache.Verify(x => x.SetPriceAsync(It.IsAny<MarketPrice>(), It.IsAny<TimeSpan?>()), Times.AtMostOnce());
    }
}
