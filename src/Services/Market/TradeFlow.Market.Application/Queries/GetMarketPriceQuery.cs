using MediatR;
using TradeFlow.Common.Application;

namespace TradeFlow.Market.Application.Queries;

public record GetMarketPriceQuery : IRequest<Result<MarketPriceDto>>
{
    public string Symbol { get; init; } = default!;
}

public record MarketPriceDto
{
    public string Symbol { get; init; } = default!;
    public string Name { get; init; } = default!;
    public string AssetType { get; init; } = default!;
    public decimal CurrentPrice { get; init; }
    public decimal PreviousClose { get; init; }
    public decimal High { get; init; }
    public decimal Low { get; init; }
    public decimal Open { get; init; }
    public decimal Volume { get; init; }
    public decimal Change { get; init; }
    public decimal ChangePercent { get; init; }
    public DateTime LastUpdated { get; init; }
}
