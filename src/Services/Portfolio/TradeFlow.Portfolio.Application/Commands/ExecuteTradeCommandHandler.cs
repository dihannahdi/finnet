using MassTransit;
using MediatR;
using TradeFlow.Common.Application;
using TradeFlow.MessageContracts.Events;
using TradeFlow.Portfolio.Domain.Entities;
using TradeFlow.Portfolio.Domain.Interfaces;

namespace TradeFlow.Portfolio.Application.Commands;

public class ExecuteTradeCommandHandler : IRequestHandler<ExecuteTradeCommand, Result<TradeDto>>
{
    private readonly IPortfolioRepository _repository;
    private readonly IPublishEndpoint _publishEndpoint;

    public ExecuteTradeCommandHandler(IPortfolioRepository repository, IPublishEndpoint publishEndpoint)
    {
        _repository = repository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Result<TradeDto>> Handle(ExecuteTradeCommand request, CancellationToken cancellationToken)
    {
        var portfolio = await _repository.GetByUserIdAsync(request.UserId, cancellationToken);
        if (portfolio is null)
        {
            portfolio = UserPortfolio.Create(request.UserId);
            await _repository.AddAsync(portfolio, cancellationToken);
        }

        try
        {
            Trade trade;
            if (request.Side == "Buy")
                trade = portfolio.ExecuteBuy(request.Symbol, request.Quantity, request.Price);
            else
                trade = portfolio.ExecuteSell(request.Symbol, request.Quantity, request.Price);

            await _repository.UpdateAsync(portfolio, cancellationToken);

            // Publish integration event
            await _publishEndpoint.Publish(new TradeExecuted
            {
                TradeId = trade.Id,
                UserId = request.UserId,
                Symbol = trade.Symbol,
                Side = trade.Side,
                Quantity = trade.Quantity,
                Price = trade.Price,
                TotalValue = trade.TotalValue,
                ExecutedAt = trade.ExecutedAt
            }, cancellationToken);

            return Result<TradeDto>.Success(new TradeDto
            {
                TradeId = trade.Id,
                Symbol = trade.Symbol,
                Side = trade.Side,
                Quantity = trade.Quantity,
                Price = trade.Price,
                TotalValue = trade.TotalValue,
                ExecutedAt = trade.ExecutedAt
            });
        }
        catch (InvalidOperationException ex)
        {
            return Result<TradeDto>.Failure(ex.Message);
        }
    }
}
