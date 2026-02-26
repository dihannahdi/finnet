namespace TradeFlow.Market.Domain.Entities;

public class MarketPrice
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Symbol { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public string AssetType { get; private set; } = default!; // Stock, Crypto
    public decimal CurrentPrice { get; private set; }
    public decimal PreviousClose { get; private set; }
    public decimal High { get; private set; }
    public decimal Low { get; private set; }
    public decimal Open { get; private set; }
    public decimal Volume { get; private set; }
    public decimal Change { get; private set; }
    public decimal ChangePercent { get; private set; }
    public DateTime LastUpdated { get; private set; } = DateTime.UtcNow;

    private MarketPrice() { }

    public static MarketPrice Create(string symbol, string name, string assetType, decimal price)
    {
        return new MarketPrice
        {
            Symbol = symbol.ToUpperInvariant(),
            Name = name,
            AssetType = assetType,
            CurrentPrice = price,
            Open = price,
            High = price,
            Low = price,
            PreviousClose = price
        };
    }

    public void UpdatePrice(decimal newPrice, decimal volume = 0)
    {
        PreviousClose = CurrentPrice;
        CurrentPrice = newPrice;
        Change = newPrice - PreviousClose;
        ChangePercent = PreviousClose != 0 ? (Change / PreviousClose) * 100 : 0;
        Volume += volume;
        if (newPrice > High) High = newPrice;
        if (newPrice < Low || Low == 0) Low = newPrice;
        LastUpdated = DateTime.UtcNow;
    }
}
