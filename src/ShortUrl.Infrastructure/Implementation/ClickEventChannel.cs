using ShortUrl.Core.Contracts;
using ShortUrl.Core.Models;
using System.Threading.Channels;

namespace ShortUrl.Infrastructure.Implementation
{
    public class ClickEventChannel : IClickEventChannel
    {
        private readonly Channel<ClickEventItem> _channel;

        public ClickEventChannel()
        {
            // BoundedChannel limita il numero di messaggi in coda per evitare memory leaks
            var options = new BoundedChannelOptions(capacity: 1000)
            {
                FullMode = BoundedChannelFullMode.Wait
            };

            _channel = Channel.CreateBounded<ClickEventItem>(options);
        }

        public ValueTask<ClickEventItem> ReadAsync(CancellationToken cancellationToken = default)
            => _channel.Reader.ReadAsync(cancellationToken);

        public ValueTask WriteAsync(ClickEventItem message, CancellationToken cancellationToken = default)
            => _channel.Writer.WriteAsync(message, cancellationToken);
    }
}
