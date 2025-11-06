using ShortUrl.Core.Models;

namespace ShortUrl.Core.Contracts
{
    public interface IClickEventService
    {
        Task LogClickEventAsync(ClickEventItem message, CancellationToken stoppingToken);
    }
}
