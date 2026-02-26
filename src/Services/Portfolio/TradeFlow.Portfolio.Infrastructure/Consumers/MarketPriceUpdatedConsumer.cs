using MassTransit;
using Microsoft.Extensions.Logging;
using TradeFlow.MessageContracts.Events;

namespace TradeFlow.Portfolio.Infrastructure.Consumers;

public class MarketPriceUpdatedConsumer : IConsumer<MarketPriceUpdated>
{
    private readonly ILogger<MarketPriceUpdatedConsumer> _logger;

    public MarketPriceUpdatedConsumer(ILogger<MarketPriceUpdatedConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<MarketPriceUpdated> context)
    {
        var msg = context.Message;
        _logger.LogDebug("Portfolio received price update: {Symbol} = {Price}", msg.Symbol, msg.Price);
        // In production, this would recalculate P&L for all portfolios holding this symbol
        return Task.CompletedTask;
    }
}
