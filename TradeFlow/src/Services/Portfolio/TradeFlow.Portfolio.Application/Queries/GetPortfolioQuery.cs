using MediatR;
using TradeFlow.Common.Application;

namespace TradeFlow.Portfolio.Application.Queries;

public record GetPortfolioQuery : IRequest<Result<PortfolioDto>>
{
    public Guid UserId { get; init; }
}

public record PortfolioDto
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public decimal CashBalance { get; init; }
    public decimal TotalValue { get; init; }
    public decimal TotalPnL { get; init; }
    public decimal TotalPnLPercent { get; init; }
    public List<PositionDto> Positions { get; init; } = new();
    public DateTime CreatedAt { get; init; }
}

public record PositionDto
{
    public Guid Id { get; init; }
    public string Symbol { get; init; } = default!;
    public decimal Quantity { get; init; }
    public decimal AveragePrice { get; init; }
    public decimal CurrentPrice { get; init; }
    public decimal MarketValue { get; init; }
    public decimal PnL { get; init; }
    public decimal PnLPercent { get; init; }
}

public record GetTradeHistoryQuery : IRequest<Result<PagedResult<TradeHistoryDto>>>
{
    public Guid UserId { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

public record TradeHistoryDto
{
    public Guid Id { get; init; }
    public string Symbol { get; init; } = default!;
    public string Side { get; init; } = default!;
    public decimal Quantity { get; init; }
    public decimal Price { get; init; }
    public decimal TotalValue { get; init; }
    public DateTime ExecutedAt { get; init; }
}
