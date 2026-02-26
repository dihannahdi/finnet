using FluentAssertions;
using TradeFlow.Market.Domain.Entities;
using Xunit;

namespace TradeFlow.Market.Tests;

public class WatchlistTests
{
    [Fact]
    public void Create_ShouldInitializeCorrectly()
    {
        var watchlist = Watchlist.Create(Guid.NewGuid(), "Tech Stocks");

        watchlist.Name.Should().Be("Tech Stocks");
        watchlist.Items.Should().BeEmpty();
    }

    [Fact]
    public void AddSymbol_ShouldAddItem()
    {
        var watchlist = Watchlist.Create(Guid.NewGuid(), "My List");
        watchlist.AddSymbol("AAPL");

        watchlist.Items.Should().HaveCount(1);
        watchlist.Items.First().Symbol.Should().Be("AAPL");
    }

    [Fact]
    public void AddSymbol_ShouldNotDuplicate()
    {
        var watchlist = Watchlist.Create(Guid.NewGuid(), "My List");
        watchlist.AddSymbol("AAPL");
        watchlist.AddSymbol("AAPL");

        watchlist.Items.Should().HaveCount(1);
    }

    [Fact]
    public void RemoveSymbol_ShouldRemoveItem()
    {
        var watchlist = Watchlist.Create(Guid.NewGuid(), "My List");
        watchlist.AddSymbol("AAPL");
        watchlist.AddSymbol("GOOGL");
        watchlist.RemoveSymbol("AAPL");

        watchlist.Items.Should().HaveCount(1);
        watchlist.Items.First().Symbol.Should().Be("GOOGL");
    }
}
