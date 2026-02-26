using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TradeFlow.Common.Domain;
using TradeFlow.Portfolio.Domain.Interfaces;
using TradeFlow.Portfolio.Infrastructure.Persistence;

namespace TradeFlow.Portfolio.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddPortfolioInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<PortfolioDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("PortfolioDb") ??
                "Host=localhost;Port=5432;Database=tradeflow_portfolio;Username=postgres;Password=postgres"));

        services.AddScoped<IPortfolioRepository, PortfolioRepository>();
        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();

        return services;
    }
}
