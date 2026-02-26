using Microsoft.EntityFrameworkCore;
using TradeFlow.Social.Domain.Entities;
using TradeFlow.Social.Domain.Interfaces;

namespace TradeFlow.Social.Infrastructure.Persistence;

public class TradeIdeaRepository : ITradeIdeaRepository
{
    private readonly SocialDbContext _context;
    public TradeIdeaRepository(SocialDbContext context) => _context = context;

    public async Task<TradeIdea?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _context.TradeIdeas.Include(i => i.Comments).Include(i => i.Likes).FirstOrDefaultAsync(i => i.Id == id, ct);

    public async Task<IReadOnlyList<TradeIdea>> GetFeedAsync(int page, int pageSize, CancellationToken ct = default) =>
        await _context.TradeIdeas.Where(i => i.IsActive).OrderByDescending(i => i.CreatedAt)
            .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);

    public async Task<IReadOnlyList<TradeIdea>> GetByAuthorAsync(Guid authorId, int page, int pageSize, CancellationToken ct = default) =>
        await _context.TradeIdeas.Where(i => i.AuthorId == authorId && i.IsActive)
            .OrderByDescending(i => i.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);

    public async Task<IReadOnlyList<TradeIdea>> GetBySymbolAsync(string symbol, int page, int pageSize, CancellationToken ct = default) =>
        await _context.TradeIdeas.Where(i => i.Symbol == symbol.ToUpperInvariant() && i.IsActive)
            .OrderByDescending(i => i.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);

    public async Task<int> GetTotalCountAsync(CancellationToken ct = default) =>
        await _context.TradeIdeas.CountAsync(i => i.IsActive, ct);

    public async Task<TradeIdea> AddAsync(TradeIdea idea, CancellationToken ct = default) { await _context.TradeIdeas.AddAsync(idea, ct); await _context.SaveChangesAsync(ct); return idea; }
    public async Task UpdateAsync(TradeIdea idea, CancellationToken ct = default) { _context.TradeIdeas.Update(idea); await _context.SaveChangesAsync(ct); }
}

public class FollowRepository : IFollowRepository
{
    private readonly SocialDbContext _context;
    public FollowRepository(SocialDbContext context) => _context = context;

    public async Task<Follow?> GetFollowAsync(Guid followerId, Guid followingId, CancellationToken ct = default) =>
        await _context.Follows.FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FollowingId == followingId, ct);

    public async Task<IReadOnlyList<Follow>> GetFollowersAsync(Guid userId, CancellationToken ct = default) =>
        await _context.Follows.Where(f => f.FollowingId == userId).ToListAsync(ct);

    public async Task<IReadOnlyList<Follow>> GetFollowingAsync(Guid userId, CancellationToken ct = default) =>
        await _context.Follows.Where(f => f.FollowerId == userId).ToListAsync(ct);

    public async Task<int> GetFollowersCountAsync(Guid userId, CancellationToken ct = default) =>
        await _context.Follows.CountAsync(f => f.FollowingId == userId, ct);

    public async Task<int> GetFollowingCountAsync(Guid userId, CancellationToken ct = default) =>
        await _context.Follows.CountAsync(f => f.FollowerId == userId, ct);

    public async Task<Follow> AddAsync(Follow follow, CancellationToken ct = default) { await _context.Follows.AddAsync(follow, ct); await _context.SaveChangesAsync(ct); return follow; }
    public async Task DeleteAsync(Follow follow, CancellationToken ct = default) { _context.Follows.Remove(follow); await _context.SaveChangesAsync(ct); }
}

public class NotificationRepository : INotificationRepository
{
    private readonly SocialDbContext _context;
    public NotificationRepository(SocialDbContext context) => _context = context;

    public async Task<IReadOnlyList<Notification>> GetByUserIdAsync(Guid userId, int page, int pageSize, CancellationToken ct = default) =>
        await _context.Notifications.Where(n => n.UserId == userId).OrderByDescending(n => n.CreatedAt)
            .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);

    public async Task<int> GetUnreadCountAsync(Guid userId, CancellationToken ct = default) =>
        await _context.Notifications.CountAsync(n => n.UserId == userId && !n.IsRead, ct);

    public async Task<Notification> AddAsync(Notification notification, CancellationToken ct = default) { await _context.Notifications.AddAsync(notification, ct); await _context.SaveChangesAsync(ct); return notification; }

    public async Task MarkAllAsReadAsync(Guid userId, CancellationToken ct = default)
    {
        await _context.Notifications.Where(n => n.UserId == userId && !n.IsRead)
            .ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, true), ct);
    }
}
