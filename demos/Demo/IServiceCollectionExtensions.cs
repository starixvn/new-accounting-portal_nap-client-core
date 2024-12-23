using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Demo;

using Microsoft.AspNetCore.DataProtection;
using StackExchange.Redis;

public static class IServiceCollectionExtensions
{
    public static void AddAuthenticationRedis(this IServiceCollection services, IConfiguration configuration)
    {
        var redisConfig = configuration.GetSection("AuthSettings:RedisConfig").Value;
        var redisMultiplexer = ConnectionMultiplexer.Connect(redisConfig);

        services.AddDataProtection()
            .PersistKeysToStackExchangeRedis(redisMultiplexer, "Fanex.Demo-ProtectionKeys");

        services.AddSingleton(redisMultiplexer);

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConfig;
            options.InstanceName = "Fanex.Demo.Session-";
        });
    }
}