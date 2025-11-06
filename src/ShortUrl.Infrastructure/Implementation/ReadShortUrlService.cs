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
        public async Task<Result<ShortUrlSearchItem>> GetLongUrl(string shortCode, CancellationToken ct)
        {
            if (!ShortUrlGenerator.IsValidShortCode(shortCode))
                Error.Validation("Invalid ShortUrl format");

            long apiKeyId = 0;

            try
            {
                var searchItem = await cache.GetOrSetAsync<ShortUrlSearchItem?>(CacheKey.ShortUrlKey(shortCode),
                    async (ctx, ct) =>
                    {
                        var entity = await context.ShortUrls.AsNoTracking()
                            .Where(su => su.ShortCode == shortCode)
                            .FirstOrDefaultAsync(ct);
                        if (entity == null || !entity.IsActive ||
                            (entity.ExpiresAt.HasValue && entity.ExpiresAt.Value < DateTime.UtcNow))
                        {
                            logger.LogWarning("ShortUrl not found or inactive/expired: {ShortUrl}", shortCode);

                            ctx.Options.Duration = TimeSpan.FromMinutes(10);
                            ctx.Options.DistributedCacheDuration = TimeSpan.FromMinutes(10);

                            return null;
                        }

                        apiKeyId = entity.OwnerId;

                        if (entity.ExpiresAt.HasValue)
                        {
                            var duration = entity.ExpiresAt.Value > DateTime.UtcNow
                                ? entity.ExpiresAt.Value - DateTime.UtcNow
                                : TimeSpan.Zero;
                            ctx.Options.Duration = duration;
                            ctx.Options.DistributedCacheDuration = duration;
                        }

                        return new ShortUrlSearchItem(entity.Id, entity.LongUrl);
                    },
                    tags: [CacheKey.TagAllShortUrl,
                        CacheKey.TagShortUrlApiKey(apiKeyId)],
                    token: ct);

                if (searchItem == null)
                {
                    return Error.NotFound("ShortUrl not found or inactive/expired");
                }

                return searchItem;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving LongUrl for ShortUrl: {ShortUrl}", shortCode);
                return Error.Failure("An error occurred while processing your request.");
            }
        }
    }
}
