using MediatR;
using TradeFlow.Common.Application;
using TradeFlow.Portfolio.Domain.Interfaces;

namespace TradeFlow.Portfolio.Application.Queries;

public class GetPortfolioQueryHandler : IRequestHandler<GetPortfolioQuery, Result<PortfolioDto>>
{
    private readonly IPortfolioRepository _repository;

    public GetPortfolioQueryHandler(IPortfolioRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<PortfolioDto>> Handle(GetPortfolioQuery request, CancellationToken cancellationToken)
    {
        var portfolio = await _repository.GetByUserIdAsync(request.UserId, cancellationToken);
        if (portfolio is null)
            return Result<PortfolioDto>.Failure("Portfolio not found. Execute a trade to create one.");

        var positions = portfolio.Positions.Select(p => new PositionDto
        {
            Id = p.Id,
            Symbol = p.Symbol,
            Quantity = p.Quantity,
            AveragePrice = p.AveragePrice,
            CurrentPrice = p.AveragePrice, // Will be enriched by caller
            MarketValue = p.Quantity * p.AveragePrice,
            PnL = 0,
            PnLPercent = 0
        }).ToList();

        var totalPositionValue = positions.Sum(p => p.MarketValue);

        return Result<PortfolioDto>.Success(new PortfolioDto
        {
            Id = portfolio.Id,
            UserId = portfolio.UserId,
            CashBalance = portfolio.CashBalance,
            TotalValue = portfolio.CashBalance + totalPositionValue,
            TotalPnL = 0,
            TotalPnLPercent = 0,
            Positions = positions,
            CreatedAt = portfolio.CreatedAt
        });
    }
}

public class GetTradeHistoryQueryHandler : IRequestHandler<GetTradeHistoryQuery, Result<PagedResult<TradeHistoryDto>>>
{
    private readonly IPortfolioRepository _repository;

    public GetTradeHistoryQueryHandler(IPortfolioRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<PagedResult<TradeHistoryDto>>> Handle(GetTradeHistoryQuery request, CancellationToken cancellationToken)
    {
        var portfolio = await _repository.GetByUserIdAsync(request.UserId, cancellationToken);
        if (portfolio is null)
            return Result<PagedResult<TradeHistoryDto>>.Failure("Portfolio not found.");

        var trades = await _repository.GetTradeHistoryAsync(portfolio.Id, request.Page, request.PageSize, cancellationToken);
        var total = await _repository.GetTradeCountAsync(portfolio.Id, cancellationToken);

        var dtos = trades.Select(t => new TradeHistoryDto
        {
            Id = t.Id, Symbol = t.Symbol, Side = t.Side,
            Quantity = t.Quantity, Price = t.Price,
            TotalValue = t.TotalValue, ExecutedAt = t.ExecutedAt
        }).ToList();

        return Result<PagedResult<TradeHistoryDto>>.Success(
            new PagedResult<TradeHistoryDto>(dtos, total, request.Page, request.PageSize));
    }
}
