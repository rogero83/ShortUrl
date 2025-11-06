using Microsoft.EntityFrameworkCore;
using ShortUrl.Common;
using ShortUrl.Core.Models;
using ShortUrl.DbPersistence;
using ZiggyCreatures.Caching.Fusion;

namespace ShortUrl.Infrastructure.ApiKeyManager
{
    public interface IApiKeyValidator
    {
        Task<ApiKeyContext?> Validate(string apiKey);
    }

    public class ApiKeyValidator(DbContextOptions<ShortUrlContext> options,
        IFusionCache cache) : IApiKeyValidator
    {
        public static FusionCacheEntryOptions InvalidApikeyCacheOptions => new()
        {
            Duration = TimeSpan.FromHours(1),
            DistributedCacheDuration = TimeSpan.FromHours(6),
        };

        public async Task<ApiKeyContext?> Validate(string apiKey)
        {
            return await cache.GetOrSetAsync<ApiKeyContext?>(CacheKey.ApiKey(apiKey), async (ctx, ct) =>
            {
                using var dbContext = new ShortUrlContext(options);
                var apiKeyEntity = await dbContext.ApiKeys
                    .AsNoTracking()
                    .FirstOrDefaultAsync(a => a.IsActive && a.ApiKey == apiKey, ct);

                if (apiKeyEntity != null)
                {
                    var context = new ApiKeyContext
                    {
                        Id = apiKeyEntity.Id,
                        ApiKey = apiKeyEntity.ApiKey,
                        CanSetCustomShortCodes = apiKeyEntity.CanSetCustomShortCodes

                    };
                    return context;
                }

                // Cache invalid keys to prevent repeated database lookups
                ctx.Options = InvalidApikeyCacheOptions;
                return null;
            }, tags: [CacheKey.TagApiKey]);
        }
    }
}
