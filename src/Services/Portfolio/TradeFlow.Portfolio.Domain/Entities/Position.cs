namespace TradeFlow.Portfolio.Domain.Entities;

public class Position
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid PortfolioId { get; private set; }
    public string Symbol { get; private set; } = default!;
    public decimal Quantity { get; private set; }
    public decimal AveragePrice { get; private set; }
    public decimal TotalCost { get; private set; }
    public DateTime OpenedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; private set; }

    private Position() { }

    public static Position Create(Guid portfolioId, string symbol, decimal quantity, decimal price)
    {
        return new Position
        {
            PortfolioId = portfolioId,
            Symbol = symbol.ToUpperInvariant(),
            Quantity = quantity,
            AveragePrice = price,
            TotalCost = quantity * price
        };
    }

    public void AddToPosition(decimal quantity, decimal price)
    {
        TotalCost += quantity * price;
        Quantity += quantity;
        AveragePrice = TotalCost / Quantity;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ReducePosition(decimal quantity)
    {
        Quantity -= quantity;
        TotalCost = Quantity * AveragePrice;
        UpdatedAt = DateTime.UtcNow;
    }
}
