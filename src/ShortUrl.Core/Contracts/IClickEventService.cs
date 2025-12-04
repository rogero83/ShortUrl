using ShortUrl.Core.Models;

namespace ShortUrl.Core.Contracts;

/// <summary>
/// Defines a contract for logging click event messages asynchronously.
/// </summary>
public interface IClickEventService
{
    /// <summary>
    /// Asynchronously logs a click event message for processing or storage.
    /// </summary>
    /// <param name="message">The click event data to be logged. Cannot be null.</param>
    /// <param name="stoppingToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous logging operation.</returns>
    Task LogClickEventAsync(ClickEventItem message, CancellationToken stoppingToken);
}
