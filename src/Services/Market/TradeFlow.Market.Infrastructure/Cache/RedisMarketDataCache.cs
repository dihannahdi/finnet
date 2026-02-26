using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using TradeFlow.Market.Domain.Entities;
using TradeFlow.Market.Domain.Interfaces;

namespace TradeFlow.Market.Infrastructure.Cache;

public class RedisMarketDataCache : IMarketDataCache
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<RedisMarketDataCache> _logger;
    private const string AllPricesKey = "market:all_symbols";

    public RedisMarketDataCache(IDistributedCache cache, ILogger<RedisMarketDataCache> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<MarketPrice?> GetPriceAsync(string symbol)
    {
        try
        {
            var json = await _cache.GetStringAsync($"market:{symbol.ToUpperInvariant()}");
            return json is null ? null : JsonSerializer.Deserialize<MarketPrice>(json);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis cache miss for {Symbol}", symbol);
            return null;
        }
    }

    public async Task SetPriceAsync(MarketPrice price, TimeSpan? expiry = null)
    {
        try
        {
            var json = JsonSerializer.Serialize(price);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiry ?? TimeSpan.FromSeconds(30)
            };
            await _cache.SetStringAsync($"market:{price.Symbol}", json, options);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to cache price for {Symbol}", price.Symbol);
        }
    }

    public async Task<IReadOnlyList<MarketPrice>> GetAllPricesAsync()
    {
        try
        {
            var json = await _cache.GetStringAsync(AllPricesKey);
            if (json is null) return Array.Empty<MarketPrice>();
            return JsonSerializer.Deserialize<List<MarketPrice>>(json) ?? new List<MarketPrice>();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis cache miss for all prices");
            return Array.Empty<MarketPrice>();
        }
    }
}
