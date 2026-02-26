using MassTransit;
using Microsoft.Extensions.Logging;
using TradeFlow.MessageContracts.Events;
using TradeFlow.Social.Domain.Entities;
using TradeFlow.Social.Domain.Interfaces;

namespace TradeFlow.Social.Infrastructure.Consumers;

public class TradeExecutedConsumer : IConsumer<TradeExecuted>
{
    private readonly INotificationRepository _notificationRepo;
    private readonly IFollowRepository _followRepo;
    private readonly ILogger<TradeExecutedConsumer> _logger;

    public TradeExecutedConsumer(INotificationRepository notificationRepo, IFollowRepository followRepo, ILogger<TradeExecutedConsumer> logger)
    {
        _notificationRepo = notificationRepo;
        _followRepo = followRepo;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<TradeExecuted> context)
    {
        var msg = context.Message;
        _logger.LogInformation("Trade executed: {UserId} {Side} {Quantity} {Symbol} @ {Price}", msg.UserId, msg.Side, msg.Quantity, msg.Symbol, msg.Price);

        var followers = await _followRepo.GetFollowersAsync(msg.UserId);
        foreach (var follower in followers)
        {
            await _notificationRepo.AddAsync(Notification.Create(
                follower.FollowerId, "Trade",
                $"Trade Alert: {msg.Symbol}",
                $"A trader you follow {msg.Side.ToLower()} {msg.Quantity} {msg.Symbol} @ ${msg.Price}",
                msg.TradeId.ToString()));
        }
    }
}
