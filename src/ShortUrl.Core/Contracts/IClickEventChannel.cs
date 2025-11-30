using ShortUrl.Core.Models;

namespace ShortUrl.Core.Contracts
{
    /// <summary>
    /// Defines a channel for asynchronously writing and reading click event items.
    /// </summary>
    /// <remarks>Implementations of this interface provide a mechanism for producers to send click event data
    /// and for consumers to receive it asynchronously. The channel may be used to decouple event producers from
    /// consumers, enabling scenarios such as event buffering or background processing.</remarks>
    public interface IClickEventChannel
    {
        /// <summary>
        /// Asynchronously writes the specified click event message to the underlying destination.
        /// </summary>
        /// <param name="message">The click event message to write. Cannot be null.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the write operation.</param>
        /// <returns>A ValueTask that represents the asynchronous write operation.</returns>
        ValueTask WriteAsync(ClickEventItem message, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously reads the next click event item from the data source.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous read operation.</param>
        /// <returns>A value task that represents the asynchronous read operation. The result contains the next <see
        /// cref="ClickEventItem"/> if available.</returns>
        ValueTask<ClickEventItem> ReadAsync(CancellationToken cancellationToken = default);
    }
}
