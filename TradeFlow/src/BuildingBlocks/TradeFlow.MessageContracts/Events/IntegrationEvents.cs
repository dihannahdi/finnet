namespace TradeFlow.MessageContracts.Events;

// Market Service publishes these
public record MarketPriceUpdated
{
    public string Symbol { get; init; } = default!;
    public decimal Price { get; init; }
    public decimal Change { get; init; }
    public decimal ChangePercent { get; init; }
    public decimal Volume { get; init; }
    public DateTime Timestamp { get; init; }
}

public record MarketTradeExecuted
{
    public string Symbol { get; init; } = default!;
    public decimal Price { get; init; }
    public decimal Volume { get; init; }
    public DateTime Timestamp { get; init; }
}

// Identity Service publishes these
public record UserRegistered
{
    public Guid UserId { get; init; }
    public string Email { get; init; } = default!;
    public string DisplayName { get; init; } = default!;
    public DateTime RegisteredAt { get; init; }
}

// Portfolio Service publishes these
public record TradeExecuted
{
    public Guid TradeId { get; init; }
    public Guid UserId { get; init; }
    public string Symbol { get; init; } = default!;
    public string Side { get; init; } = default!; // Buy/Sell
    public decimal Quantity { get; init; }
    public decimal Price { get; init; }
    public decimal TotalValue { get; init; }
    public DateTime ExecutedAt { get; init; }
}

public record PortfolioUpdated
{
    public Guid UserId { get; init; }
    public decimal TotalValue { get; init; }
    public decimal TotalPnL { get; init; }
    public decimal TotalPnLPercent { get; init; }
    public DateTime UpdatedAt { get; init; }
}

// Social Service publishes these
public record TradeIdeaPublished
{
    public Guid IdeaId { get; init; }
    public Guid AuthorId { get; init; }
    public string Symbol { get; init; } = default!;
    public string Direction { get; init; } = default!;
    public string Content { get; init; } = default!;
    public DateTime PublishedAt { get; init; }
}

public record PriceAlertTriggered
{
    public Guid AlertId { get; init; }
    public Guid UserId { get; init; }
    public string Symbol { get; init; } = default!;
    public decimal TargetPrice { get; init; }
    public decimal CurrentPrice { get; init; }
    public string Direction { get; init; } = default!; // Above/Below
    public DateTime TriggeredAt { get; init; }
}
