using ShortUrl.Core.Models;

namespace ShortUrl.Core.Contracts
{
    public interface IClickEventChannel
    {
        ValueTask WriteAsync(ClickEventItem message, CancellationToken cancellationToken = default);
        ValueTask<ClickEventItem> ReadAsync(CancellationToken cancellationToken = default);
    }
}
