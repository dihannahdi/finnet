namespace TradeFlow.Market.Domain.Entities;

public class Watchlist
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid UserId { get; private set; }
    public string Name { get; private set; } = default!;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    
    private readonly List<WatchlistItem> _items = new();
    public IReadOnlyCollection<WatchlistItem> Items => _items.AsReadOnly();

    private Watchlist() { }

    public static Watchlist Create(Guid userId, string name)
    {
        return new Watchlist { UserId = userId, Name = name };
    }

    public void AddSymbol(string symbol)
    {
        if (_items.Any(i => i.Symbol == symbol.ToUpperInvariant())) return;
        _items.Add(WatchlistItem.Create(Id, symbol));
    }

    public void RemoveSymbol(string symbol)
    {
        var item = _items.FirstOrDefault(i => i.Symbol == symbol.ToUpperInvariant());
        if (item != null) _items.Remove(item);
    }
}

public class WatchlistItem
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid WatchlistId { get; private set; }
    public string Symbol { get; private set; } = default!;
    public DateTime AddedAt { get; private set; } = DateTime.UtcNow;

    private WatchlistItem() { }

    public static WatchlistItem Create(Guid watchlistId, string symbol)
    {
        return new WatchlistItem { WatchlistId = watchlistId, Symbol = symbol.ToUpperInvariant() };
    }
}
