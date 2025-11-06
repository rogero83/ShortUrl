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
                var entity = new ClickEventEntity
                {
                    ShortUrlId = message.ShortUrlId,
                    IpAddress = message.IpAddress,
                    UserAgent = message.UserAgent,
                    Referrer = message.Referrer,
                    ClickedAt = DateTime.UtcNow
                };
                await context.ClickEvents.AddAsync(entity, stoppingToken);
                await context.SaveChangesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to log ClickEvent: {ClickEvent}", message);
            }
        }
    }
}
