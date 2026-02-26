using Microsoft.EntityFrameworkCore;
using TradeFlow.Market.Domain.Entities;
using TradeFlow.Market.Domain.Interfaces;

namespace TradeFlow.Market.Infrastructure.Persistence;

public class MarketPriceRepository : IMarketPriceRepository
{
    private readonly MarketDbContext _context;
    public MarketPriceRepository(MarketDbContext context) => _context = context;

    public async Task<MarketPrice?> GetBySymbolAsync(string symbol, CancellationToken ct = default) =>
        await _context.MarketPrices.FirstOrDefaultAsync(p => p.Symbol == symbol.ToUpperInvariant(), ct);

    public async Task<IReadOnlyList<MarketPrice>> GetAllAsync(CancellationToken ct = default) =>
        await _context.MarketPrices.OrderBy(p => p.Symbol).ToListAsync(ct);

    public async Task<IReadOnlyList<MarketPrice>> SearchAsync(string query, CancellationToken ct = default) =>
        await _context.MarketPrices
            .Where(p => p.Symbol.Contains(query.ToUpperInvariant()) || p.Name.ToLower().Contains(query.ToLower()))
            .Take(20).ToListAsync(ct);

    public async Task<MarketPrice> AddAsync(MarketPrice price, CancellationToken ct = default)
    {
        await _context.MarketPrices.AddAsync(price, ct);
        await _context.SaveChangesAsync(ct);
        return price;
    }

    public async Task UpdateAsync(MarketPrice price, CancellationToken ct = default)
    {
        _context.MarketPrices.Update(price);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpsertAsync(MarketPrice price, CancellationToken ct = default)
    {
        var existing = await GetBySymbolAsync(price.Symbol, ct);
        if (existing is null)
            await AddAsync(price, ct);
        else
            await UpdateAsync(existing, ct);
    }
}

public class WatchlistRepository : IWatchlistRepository
{
    private readonly MarketDbContext _context;
    public WatchlistRepository(MarketDbContext context) => _context = context;

    public async Task<Watchlist?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _context.Watchlists.Include(w => w.Items).FirstOrDefaultAsync(w => w.Id == id, ct);

    public async Task<IReadOnlyList<Watchlist>> GetByUserIdAsync(Guid userId, CancellationToken ct = default) =>
        await _context.Watchlists.Include(w => w.Items).Where(w => w.UserId == userId).ToListAsync(ct);

    public async Task<Watchlist> AddAsync(Watchlist watchlist, CancellationToken ct = default)
    {
        await _context.Watchlists.AddAsync(watchlist, ct);
        await _context.SaveChangesAsync(ct);
        return watchlist;
    }

    public async Task UpdateAsync(Watchlist watchlist, CancellationToken ct = default)
    {
        _context.Watchlists.Update(watchlist);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Watchlist watchlist, CancellationToken ct = default)
    {
        _context.Watchlists.Remove(watchlist);
        await _context.SaveChangesAsync(ct);
    }
}
