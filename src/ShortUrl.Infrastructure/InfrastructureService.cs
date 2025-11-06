using FluentValidation;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ShortUrl.Core.Contracts;
using ShortUrl.Core.Models;
using ShortUrl.DbPersistence;
using ShortUrl.Infrastructure.ApiKeyManager;
using ShortUrl.Infrastructure.Implementation;
using ShortUrl.Infrastructure.Validators;
using StackExchange.Redis;
using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Backplane.StackExchangeRedis;
using ZiggyCreatures.Caching.Fusion.Serialization.SystemTextJson;

namespace ShortUrl.Infrastructure
{
    public static class InfrastructureService
    {
        public static void AddInfrastructureService(this IHostApplicationBuilder builder)
        {
            builder.AddShortUrlDbPersistenceIntegration();

            builder.AddRedisClient("redis");

            // FusionCahe registration
            builder.Services.AddMemoryCache();
            builder.Services.AddSingleton<FusionCacheSystemTextJsonSerializer>();
            builder.Services.AddFusionCache()
                .WithRegisteredMemoryCache()
                .WithDefaultEntryOptions(new FusionCacheEntryOptions
                {
                    IsFailSafeEnabled = false,
                    EagerRefreshThreshold = null,
                    FailSafeMaxDuration = TimeSpan.Zero,
                    FailSafeThrottleDuration = TimeSpan.Zero,
                    DistributedCacheFailSafeMaxDuration = TimeSpan.Zero,
                    // Cache duration inMemory
                    Duration = TimeSpan.FromHours(12),
                    // Cache duration in distributed cache
                    DistributedCacheDuration = TimeSpan.FromDays(1),
                })
                .WithSerializer(sp => sp.GetRequiredService<FusionCacheSystemTextJsonSerializer>())
                .WithDistributedCache(sp =>
                {
                    var multiplexer = sp.GetRequiredService<IConnectionMultiplexer>();
                    return new RedisCache(new RedisCacheOptions
                    {
                        ConnectionMultiplexerFactory = () => Task.FromResult(multiplexer)
                    });
                })
                .WithBackplane(sp =>
                {
                    var multiplexer = sp.GetRequiredService<IConnectionMultiplexer>();
                    return new RedisBackplane(new RedisBackplaneOptions
                    {
                        ConnectionMultiplexerFactory = () => Task.FromResult(multiplexer)
                    });
                });

            builder.Services.AddScoped<IReadShortUrlService, ReadShortUrlService>();
            builder.Services.AddScoped<IShortUrlService, ShortUrlService>();
            builder.Services.AddScoped<IClickEventService, ClickEventService>();

            builder.Services.AddSingleton<IClickEventChannel, ClickEventChannel>();

            // ApiKey Validator
            builder.Services.AddSingleton<IApiKeyValidator, ApiKeyValidator>();
            builder.Services.AddSingleton<IApiKeyContextAccessor, ApiKeyContextAccessor>();

            // Add FluentValidarors
            builder.Services.AddScoped<IValidator<CreateShortUrlRequest>, CreateShortUrlRequestValidator>();
        }
    }
}
