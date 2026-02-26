using MediatR;
using TradeFlow.Common.Application;

namespace TradeFlow.Market.Application.Queries;

public record GetAllPricesQuery : IRequest<Result<IReadOnlyList<MarketPriceDto>>>;

public record SearchMarketQuery : IRequest<Result<IReadOnlyList<MarketPriceDto>>>
{
    public string Query { get; init; } = default!;
}
