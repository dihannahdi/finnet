using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TradeFlow.Common.Domain;
using TradeFlow.Identity.Domain.Interfaces;
using TradeFlow.Identity.Infrastructure.Persistence;
using TradeFlow.Identity.Infrastructure.Services;

namespace TradeFlow.Identity.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<IdentityDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("IdentityDb") ??
                "Host=localhost;Port=5432;Database=tradeflow_identity;Username=postgres;Password=postgres"));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();
        services.AddHttpClient();

        return services;
    }
}
