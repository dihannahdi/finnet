using TradeFlow.Market.Domain.Entities;

namespace TradeFlow.Market.Domain.Interfaces;

public interface IMarketPriceRepository
{
    Task<MarketPrice?> GetBySymbolAsync(string symbol, CancellationToken ct = default);
    Task<IReadOnlyList<MarketPrice>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<MarketPrice>> SearchAsync(string query, CancellationToken ct = default);
    Task<MarketPrice> AddAsync(MarketPrice price, CancellationToken ct = default);
    Task UpdateAsync(MarketPrice price, CancellationToken ct = default);
    Task UpsertAsync(MarketPrice price, CancellationToken ct = default);
}

public interface IWatchlistRepository
{
    Task<Watchlist?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Watchlist>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task<Watchlist> AddAsync(Watchlist watchlist, CancellationToken ct = default);
    Task UpdateAsync(Watchlist watchlist, CancellationToken ct = default);
    Task DeleteAsync(Watchlist watchlist, CancellationToken ct = default);
}

public interface IMarketDataCache
{
    Task<MarketPrice?> GetPriceAsync(string symbol);
    Task SetPriceAsync(MarketPrice price, TimeSpan? expiry = null);
    Task<IReadOnlyList<MarketPrice>> GetAllPricesAsync();
}
