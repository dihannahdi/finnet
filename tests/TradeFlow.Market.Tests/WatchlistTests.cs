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
    public void Create_ShouldSetUserId()
    {
        var userId = Guid.NewGuid();
        var watchlist = Watchlist.Create(userId, "Test");

        watchlist.UserId.Should().Be(userId);
        watchlist.Id.Should().NotBeEmpty();
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
    public void AddSymbol_ShouldNormalizeToUpperCase()
    {
        var watchlist = Watchlist.Create(Guid.NewGuid(), "My List");
        watchlist.AddSymbol("aapl");

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
    public void AddSymbol_MultipleSymbols_ShouldAddAll()
    {
        var watchlist = Watchlist.Create(Guid.NewGuid(), "Tech");
        watchlist.AddSymbol("AAPL");
        watchlist.AddSymbol("GOOGL");
        watchlist.AddSymbol("MSFT");

        watchlist.Items.Should().HaveCount(3);
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

    [Fact]
    public void RemoveSymbol_ShouldDoNothing_WhenSymbolNotFound()
    {
        var watchlist = Watchlist.Create(Guid.NewGuid(), "My List");
        watchlist.AddSymbol("AAPL");
        watchlist.RemoveSymbol("GOOGL");

        watchlist.Items.Should().HaveCount(1);
    }

    [Fact]
    public void WatchlistItem_Create_ShouldNormalizeSymbol()
    {
        var item = WatchlistItem.Create(Guid.NewGuid(), "msft");
        item.Symbol.Should().Be("MSFT");
    }
}
