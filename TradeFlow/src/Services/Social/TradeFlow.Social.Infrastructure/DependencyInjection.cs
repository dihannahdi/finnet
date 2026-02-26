using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TradeFlow.Social.Domain.Interfaces;
using TradeFlow.Social.Infrastructure.Persistence;

namespace TradeFlow.Social.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddSocialInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<SocialDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("SocialDb") ??
                "Host=localhost;Port=5432;Database=tradeflow_social;Username=postgres;Password=postgres"));

        services.AddScoped<ITradeIdeaRepository, TradeIdeaRepository>();
        services.AddScoped<IFollowRepository, FollowRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();

        return services;
    }
}
