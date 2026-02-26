using MediatR;
using TradeFlow.Common.Application;
using TradeFlow.Market.Domain.Interfaces;

namespace TradeFlow.Market.Application.Queries;

public class GetAllPricesQueryHandler : IRequestHandler<GetAllPricesQuery, Result<IReadOnlyList<MarketPriceDto>>>
{
    private readonly IMarketDataCache _cache;
    private readonly IMarketPriceRepository _repository;

    public GetAllPricesQueryHandler(IMarketDataCache cache, IMarketPriceRepository repository)
    {
        _cache = cache;
        _repository = repository;
    }

    public async Task<Result<IReadOnlyList<MarketPriceDto>>> Handle(GetAllPricesQuery request, CancellationToken cancellationToken)
    {
        var prices = await _cache.GetAllPricesAsync();
        if (prices.Count == 0)
            prices = await _repository.GetAllAsync(cancellationToken);

        var dtos = prices.Select(p => new MarketPriceDto
        {
            Symbol = p.Symbol, Name = p.Name, AssetType = p.AssetType,
            CurrentPrice = p.CurrentPrice, PreviousClose = p.PreviousClose,
            High = p.High, Low = p.Low, Open = p.Open, Volume = p.Volume,
            Change = p.Change, ChangePercent = p.ChangePercent, LastUpdated = p.LastUpdated
        }).ToList();

        return Result<IReadOnlyList<MarketPriceDto>>.Success(dtos);
    }
}

public class SearchMarketQueryHandler : IRequestHandler<SearchMarketQuery, Result<IReadOnlyList<MarketPriceDto>>>
{
    private readonly IMarketPriceRepository _repository;

    public SearchMarketQueryHandler(IMarketPriceRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<IReadOnlyList<MarketPriceDto>>> Handle(SearchMarketQuery request, CancellationToken cancellationToken)
    {
        var prices = await _repository.SearchAsync(request.Query, cancellationToken);
        var dtos = prices.Select(p => new MarketPriceDto
        {
            Symbol = p.Symbol, Name = p.Name, AssetType = p.AssetType,
            CurrentPrice = p.CurrentPrice, PreviousClose = p.PreviousClose,
            High = p.High, Low = p.Low, Open = p.Open, Volume = p.Volume,
            Change = p.Change, ChangePercent = p.ChangePercent, LastUpdated = p.LastUpdated
        }).ToList();

        return Result<IReadOnlyList<MarketPriceDto>>.Success(dtos);
    }
}
