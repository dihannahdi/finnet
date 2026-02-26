namespace TradeFlow.Portfolio.Domain.Entities;

public class UserPortfolio
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid UserId { get; private set; }
    public decimal CashBalance { get; private set; } = 100000m; // Start with $100k demo cash
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; private set; }

    private readonly List<Position> _positions = new();
    public IReadOnlyCollection<Position> Positions => _positions.AsReadOnly();

    private readonly List<Trade> _trades = new();
    public IReadOnlyCollection<Trade> Trades => _trades.AsReadOnly();

    private UserPortfolio() { }

    public static UserPortfolio Create(Guid userId)
    {
        return new UserPortfolio { UserId = userId };
    }

    public Trade ExecuteBuy(string symbol, decimal quantity, decimal price)
    {
        var totalCost = quantity * price;
        if (totalCost > CashBalance)
            throw new InvalidOperationException("Insufficient cash balance.");

        CashBalance -= totalCost;
        
        var position = _positions.FirstOrDefault(p => p.Symbol == symbol.ToUpperInvariant());
        if (position != null)
        {
            position.AddToPosition(quantity, price);
        }
        else
        {
            position = Position.Create(Id, symbol, quantity, price);
            _positions.Add(position);
        }

        var trade = Trade.Create(Id, symbol, "Buy", quantity, price);
        _trades.Add(trade);
        UpdatedAt = DateTime.UtcNow;
        return trade;
    }

    public Trade ExecuteSell(string symbol, decimal quantity, decimal price)
    {
        var position = _positions.FirstOrDefault(p => p.Symbol == symbol.ToUpperInvariant());
        if (position is null || position.Quantity < quantity)
            throw new InvalidOperationException("Insufficient position quantity.");

        CashBalance += quantity * price;
        position.ReducePosition(quantity);

        if (position.Quantity == 0)
            _positions.Remove(position);

        var trade = Trade.Create(Id, symbol, "Sell", quantity, price);
        _trades.Add(trade);
        UpdatedAt = DateTime.UtcNow;
        return trade;
    }

    public decimal GetTotalValue(Func<string, decimal> getCurrentPrice)
    {
        var positionValue = _positions.Sum(p => p.Quantity * getCurrentPrice(p.Symbol));
        return CashBalance + positionValue;
    }

    public decimal GetTotalPnL(Func<string, decimal> getCurrentPrice)
    {
        return _positions.Sum(p => (getCurrentPrice(p.Symbol) - p.AveragePrice) * p.Quantity);
    }
}
