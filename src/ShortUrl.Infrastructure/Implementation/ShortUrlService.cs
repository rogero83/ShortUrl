using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShortUrl.Common;
using ShortUrl.Core.Contracts;
using ShortUrl.Core.Enums;
using ShortUrl.Core.Models;
using ShortUrl.DbPersistence;
using ShortUrl.Entities;
using ZiggyCreatures.Caching.Fusion;

namespace ShortUrl.Infrastructure.Implementation
{
    public class ShortUrlService(ShortUrlContext dbContext,
        IFusionCache cache,
        ILogger<ShortUrlService> logger
        ) : IShortUrlService
    {
        private const int MaxStep = 5;

        public async Task<Result<CreateShortUrlResponse>> CreateShortUrl(ApiKeyContext apiKeyContext,
            CreateShortUrlRequest request,
            CancellationToken ct)
        {
            var step = 0;
            do
            {
                var shortCode = request.ShortCode ?? ShortUrlGenerator.GenerateShortCode();
                if (!await dbContext.ShortUrls.AnyAsync(su => su.ShortCode == shortCode, ct))
                {
                    var shortUrl = ShortUrlEntity.Create(
                        shortCode,
                        apiKeyContext.Id,
                        request.OriginalUrl,
                        request.Expire,
                        request.Activate);

                    if (shortUrl.IsFailure)
                        return shortUrl.Error;

                    await dbContext.ShortUrls.AddAsync(shortUrl.Value, ct);
                    await dbContext.SaveChangesAsync(ct);
                    await SetCacheItem(shortUrl.Value, ct);

                    logger.LogInformation("Created ShortUrl: {ShortCode} for ApiKeyId: {ApiKeyId}", shortCode, apiKeyContext.Id);

                    var response = new CreateShortUrlResponse(shortCode);
                    return response;
                }

                step++;
            } while (step > MaxStep);

            logger.LogError("Failed to generate a unique short URL after {MaxStep} attempts for ApiKeyId: {ApiKeyId}", MaxStep, apiKeyContext.Id);
            return Error.Failure("Unable to generate a unique short URL after multiple attempts.");
        }

        public async Task<Result<EditShortUrlResponse>> EditShortUrl(ApiKeyContext apiKeyContext, string shortCode, EditShortUrlRequest request, CancellationToken ct)
        {
            // Validators
            if (shortCode == null || !ShortUrlGenerator.IsValidShortCode(shortCode))
            {
                logger.LogWarning("Invalid ShortCode format provided by ApiKeyId: {ApiKeyId}, ShortCode: {ShortCode}", apiKeyContext.Id, shortCode);
                return Error.Validation("The provided short URL code is not valid.");
            }

            // Retrieve existing ShortUrl
            var shortUrlEntity = await dbContext.ShortUrls
                .FirstOrDefaultAsync(su => su.ShortCode == shortCode && su.OwnerId == apiKeyContext.Id, ct);
            if (shortUrlEntity == null)
            {
                logger.LogWarning("ShortUrl not found for editing by ApiKeyId: {ApiKeyId}, ShortCode: {ShortCode}", apiKeyContext.Id, shortCode);
                return Error.NotFound("The specified short URL was not found.");
            }

            // Update fields
            var editAction = shortUrlEntity.Edit(request.OriginalUrl,
                request.Expire,
                request.IsActive);

            if (editAction.IsFailure)
                return editAction.Error;

            await dbContext.SaveChangesAsync(ct);

            await SetCacheItem(shortUrlEntity, ct);

            return new EditShortUrlResponse(shortUrlEntity.ShortCode);
        }

        public async Task<Result<ShortCodesResponse>> ListShortUrls(ApiKeyContext current, ShortCodesRequest request, CancellationToken ct)
        {
            var query = dbContext.ShortUrls
                .Where(su => su.OwnerId == current.Id);

            var totalItems = await query.CountAsync(ct);

            switch (request.OrderField)
            {
                case OrderType.CreatedAtDesc:
                    query = query.OrderByDescending(su => su.CreatedAt);
                    break;
                case OrderType.CreatedAtAsc:
                    query = query.OrderBy(su => su.CreatedAt);
                    break;
                case OrderType.ExpireAtDesc:
                    query = query.OrderByDescending(su => su.ExpiresAt);
                    break;
                case OrderType.ExpireAtAsc:
                    query = query.OrderBy(su => su.ExpiresAt);
                    break;
                case OrderType.TotalClicksDesc:
                    query = query.OrderByDescending(su => su.TotalClicks);
                    break;
                case OrderType.TotalClicksAsc:
                    query = query.OrderBy(su => su.TotalClicks);
                    break;
                case OrderType.UniqueClicksDesc:
                    query = query.OrderByDescending(su => su.UniqueClicks);
                    break;
                case OrderType.UniqueClicksAsc:
                    query = query.OrderBy(su => su.UniqueClicks);
                    break;
                default:
                    return Error.Validation("Invalid order field specified.");
            }

            var shortUrls = await query
                .Skip((request.Page - 1) * request.ItemByPage)
                .Take(request.ItemByPage)
                .Select(su => new ShortUrlInfo
                {
                    ShortCode = su.ShortCode,
                    LongUrl = su.LongUrl,
                    IsActive = su.IsActive,
                    TotalClicks = su.TotalClicks,
                    UniqueClicks = su.UniqueClicks,
                    ExpireAt = su.ExpiresAt,
                    CreatedAt = su.CreatedAt
                })
                .ToListAsync(ct);

            return new ShortCodesResponse
            {
                Items = shortUrls,
                Page = request.Page,
                ItemsByPage = request.ItemByPage,
                TotalItems = totalItems
            };
        }

        private async Task SetCacheItem(ShortUrlEntity shortUrl, CancellationToken ct)
        {
            await cache.SetAsync<ShortUrlSearchItem>(CacheKey.ShortUrlKey(shortUrl.ShortCode),
                new ShortUrlSearchItem(shortUrl.Id, shortUrl.LongUrl),
                (opt) =>
                {
                    if (shortUrl.ExpiresAt.HasValue)
                    {
                        var duration = shortUrl.ExpiresAt.Value > DateTime.UtcNow
                            ? shortUrl.ExpiresAt.Value - DateTime.UtcNow
                            : TimeSpan.Zero;
                        opt.Duration = duration;
                        opt.DistributedCacheDuration = duration;
                    }
                },
            tags: [CacheKey.TagAllShortUrl],
            token: ct);
        }
    }
}
