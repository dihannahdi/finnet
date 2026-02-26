using TradeFlow.Social.Domain.Entities;

namespace TradeFlow.Social.Domain.Interfaces;

public interface ITradeIdeaRepository
{
    Task<TradeIdea?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<TradeIdea>> GetFeedAsync(int page, int pageSize, CancellationToken ct = default);
    Task<IReadOnlyList<TradeIdea>> GetByAuthorAsync(Guid authorId, int page, int pageSize, CancellationToken ct = default);
    Task<IReadOnlyList<TradeIdea>> GetBySymbolAsync(string symbol, int page, int pageSize, CancellationToken ct = default);
    Task<int> GetTotalCountAsync(CancellationToken ct = default);
    Task<TradeIdea> AddAsync(TradeIdea idea, CancellationToken ct = default);
    Task UpdateAsync(TradeIdea idea, CancellationToken ct = default);
}

public interface IFollowRepository
{
    Task<Follow?> GetFollowAsync(Guid followerId, Guid followingId, CancellationToken ct = default);
    Task<IReadOnlyList<Follow>> GetFollowersAsync(Guid userId, CancellationToken ct = default);
    Task<IReadOnlyList<Follow>> GetFollowingAsync(Guid userId, CancellationToken ct = default);
    Task<int> GetFollowersCountAsync(Guid userId, CancellationToken ct = default);
    Task<int> GetFollowingCountAsync(Guid userId, CancellationToken ct = default);
    Task<Follow> AddAsync(Follow follow, CancellationToken ct = default);
    Task DeleteAsync(Follow follow, CancellationToken ct = default);
}

public interface INotificationRepository
{
    Task<IReadOnlyList<Notification>> GetByUserIdAsync(Guid userId, int page, int pageSize, CancellationToken ct = default);
    Task<int> GetUnreadCountAsync(Guid userId, CancellationToken ct = default);
    Task<Notification> AddAsync(Notification notification, CancellationToken ct = default);
    Task MarkAllAsReadAsync(Guid userId, CancellationToken ct = default);
}
