using ShortUrl.Common;
using ShortUrl.Core.Models;

namespace ShortUrl.Core.Contracts;

/// <summary>
/// Defines methods for retrieving and checking the existence of shortened URLs.
/// </summary>
/// <remarks>Implementations of this interface provide read-only access to short URL data, typically used to
/// resolve short codes to their original URLs or to verify if a short code exists. Methods are asynchronous and support
/// cancellation via a CancellationToken.</remarks>
public interface IReadShortUrlService
{
    /// <summary>
    /// Retrieves the original long URL associated with the specified short code.
    /// </summary>
    /// <param name="shortCode">The short code representing the shortened URL to look up. Cannot be null or empty.</param>
    /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see
    /// cref="Result{ShortUrlSearchItem}"/> with the long URL information if found; otherwise, an error result.</returns>
    Task<Result<ShortUrlSearchItem>> GetLongUrl(string shortCode, CancellationToken ct);

    /// <summary>
    /// Determines whether a resource with the specified short code exists.
    /// </summary>
    /// <param name="shortCode">The short code that identifies the resource to check for existence. Cannot be null or empty.</param>
    /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains <see langword="true"/> if the
    /// resource exists; otherwise, <see langword="false"/>.</returns>
    Task<bool> Exists(string shortCode, CancellationToken ct);
}
