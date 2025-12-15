using Microsoft.Extensions.Logging;
using ShortUrl.Core.Contracts;
using ShortUrl.Core.Models;
using ShortUrl.DbPersistence;
using ShortUrl.Entities;

namespace ShortUrl.Infrastructure.Implementation
{
    public class ClickEventService(ShortUrlContext context,
        ILogger<ClickEventService> logger)
        : IClickEventService
    {
        public async Task LogClickEventAsync(ClickEventItem message, CancellationToken stoppingToken)
        {
            try
            {
                var entity = ClickEventEntity.Create(
                    message.ShortUrlId,
                    message.IpAddress,
                    message.UserAgent,
                    message.Referrer,
                    message.ClickedAt);

                await context.ClickEvents.AddAsync(entity.Value, stoppingToken);
                await context.SaveChangesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to log ClickEvent: {ClickEvent}", message);
            }
        }
    }
}
