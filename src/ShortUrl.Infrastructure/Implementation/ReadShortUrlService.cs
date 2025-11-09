using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShortUrl.Common;
using ShortUrl.Common.ResultPattern;
using ShortUrl.Core.Contracts;
using ShortUrl.Core.Models;
using ShortUrl.DbPersistence;
using ZiggyCreatures.Caching.Fusion;

namespace ShortUrl.Infrastructure.Implementation
{
    public class ReadShortUrlService(IFusionCache cache,
        ShortUrlContext context,
        ILogger<ReadShortUrlService> logger)
        : IReadShortUrlService
    {
        private readonly TimeSpan negativeTimeStamp = TimeSpan.FromMinutes(10);

        public async Task<Result<ShortUrlSearchItem>> GetLongUrl(string shortCode, CancellationToken ct)
        {
            if (!ShortUrlGenerator.IsValidShortCode(shortCode))
            {
                return Error.Validation("Invalid ShortUrl format");
            }

            var cacheKey = CacheKey.ShortUrlKey(shortCode);

            try
            {
                // Uset Get Or Set to attempt to avoid cache stampede
                var maybeItem = await cache.GetOrSetAsync<ShortUrlSearchItem?>(cacheKey, async (ctx, ct) =>
                {
                    var entity = await context.ShortUrls.AsNoTracking()
                        .Where(su => su.ShortCode == shortCode)
                        .FirstOrDefaultAsync(ct);

                    // Not found, inactive, or expired
                    if (entity == null || !entity.IsActive || (entity.ExpiresAt.HasValue && entity.ExpiresAt.Value < DateTime.UtcNow))
                    {
                        logger.LogWarning("ShortUrl not found or inactive/expired: {ShortUrl}", shortCode);
                        // Set a negative cache entry to prevent hammering the DB for non-existent URLs
                        ctx.Options
                            .SetDuration(negativeTimeStamp)
                            .SetDistributedCacheDuration(negativeTimeStamp);

                        return null;
                    }

                    // Found: create item and cache it with the correct tags and duration
                    TimeSpan? duration = null;
                    if (entity.ExpiresAt.HasValue)
                    {
                        duration = entity.ExpiresAt.Value > DateTime.UtcNow
                            ? entity.ExpiresAt.Value - DateTime.UtcNow
                            : TimeSpan.Zero;

                        if (duration <= TimeSpan.Zero)
                        {
                            // If for some reason it's already expired, treat as not found
                            logger.LogWarning("ShortUrl found but already expired: {ShortUrl}", shortCode);
                            ctx.Options
                                .SetDuration(negativeTimeStamp)
                                .SetDistributedCacheDuration(negativeTimeStamp);

                            return null;
                        }
                    }

                    var itemToCache = new ShortUrlSearchItem(entity.Id, entity.LongUrl);

                    if (duration.HasValue)
                    {
                        // We've already checked for duration > Zero above                            
                        ctx.Options
                            .SetDuration(duration.Value)
                            .SetDistributedCacheDuration(duration.Value);
                    }

                    return itemToCache;
                },
                tags: [CacheKey.TagAllShortUrl],
                token: ct);

                return maybeItem != null
                    ? Result<ShortUrlSearchItem>.Success(maybeItem)
                    : Error.NotFound("ShortUrl not found or inactive/expired");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving LongUrl for ShortUrl: {ShortUrl}", shortCode);
                return Error.Failure("An error occurred while processing your request.");
            }
        }

        public async Task<bool> Exists(string shortCode, CancellationToken ct)
        {
            // Use GetLongUrl to leverage caching logic
            var result = await GetLongUrl(shortCode, ct);
            return result.IsSuccess;
        }
    }
}
