using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TradeFlow.Common.Domain;
using TradeFlow.Market.Domain.Interfaces;
using TradeFlow.Market.Infrastructure.Cache;
using TradeFlow.Market.Infrastructure.Persistence;
using TradeFlow.Market.Infrastructure.Services;

namespace TradeFlow.Market.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddMarketInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<MarketDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("MarketDb") ??
                "Host=localhost;Port=5432;Database=tradeflow_market;Username=postgres;Password=postgres"));

        services.AddScoped<IMarketPriceRepository, MarketPriceRepository>();
        services.AddScoped<IWatchlistRepository, WatchlistRepository>();
        services.AddScoped<IMarketDataCache, RedisMarketDataCache>();

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis") ?? "localhost:6379";
            options.InstanceName = "TradeFlow:Market:";
        });

        services.AddHostedService<FinnhubWebSocketService>();
        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();

        return services;
    }
}
