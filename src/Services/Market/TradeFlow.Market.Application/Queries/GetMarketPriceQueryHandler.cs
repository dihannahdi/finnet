using MediatR;
using TradeFlow.Common.Application;
using TradeFlow.Market.Domain.Interfaces;

namespace TradeFlow.Market.Application.Queries;

public class GetMarketPriceQueryHandler : IRequestHandler<GetMarketPriceQuery, Result<MarketPriceDto>>
{
    private readonly IMarketDataCache _cache;
    private readonly IMarketPriceRepository _repository;

    public GetMarketPriceQueryHandler(IMarketDataCache cache, IMarketPriceRepository repository)
    {
        _cache = cache;
        _repository = repository;
    }

    public async Task<Result<MarketPriceDto>> Handle(GetMarketPriceQuery request, CancellationToken cancellationToken)
    {
        var cached = await _cache.GetPriceAsync(request.Symbol);
        if (cached != null)
            return Result<MarketPriceDto>.Success(MapToDto(cached));

        var price = await _repository.GetBySymbolAsync(request.Symbol, cancellationToken);
        if (price is null)
            return Result<MarketPriceDto>.Failure($"Symbol {request.Symbol} not found.");

        await _cache.SetPriceAsync(price, TimeSpan.FromSeconds(30));
        return Result<MarketPriceDto>.Success(MapToDto(price));
    }

    private static MarketPriceDto MapToDto(TradeFlow.Market.Domain.Entities.MarketPrice p) => new()
    {
        Symbol = p.Symbol,
        Name = p.Name,
        AssetType = p.AssetType,
        CurrentPrice = p.CurrentPrice,
        PreviousClose = p.PreviousClose,
        High = p.High,
        Low = p.Low,
        Open = p.Open,
        Volume = p.Volume,
        Change = p.Change,
        ChangePercent = p.ChangePercent,
        LastUpdated = p.LastUpdated
    };
}
