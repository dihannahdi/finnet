namespace TradeFlow.Portfolio.Domain.Entities;

public class Trade
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid PortfolioId { get; private set; }
    public string Symbol { get; private set; } = default!;
    public string Side { get; private set; } = default!;
    public decimal Quantity { get; private set; }
    public decimal Price { get; private set; }
    public decimal TotalValue { get; private set; }
    public DateTime ExecutedAt { get; private set; } = DateTime.UtcNow;

    private Trade() { }

    public static Trade Create(Guid portfolioId, string symbol, string side, decimal quantity, decimal price)
    {
        return new Trade
        {
            PortfolioId = portfolioId,
            Symbol = symbol.ToUpperInvariant(),
            Side = side,
            Quantity = quantity,
            Price = price,
            TotalValue = quantity * price
        };
    }
}
