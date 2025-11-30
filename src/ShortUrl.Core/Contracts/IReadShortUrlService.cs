using ShortUrl.Common.ResultPattern;
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
    /// Get shortUrl from Cache or Database
    /// </summary>
    /// <param name="shortCode"></param>
    /// <returns></returns>
    Task<Result<ShortUrlSearchItem>> GetLongUrl(string shortCode, CancellationToken ct);

    Task<bool> Exists(string shortCode, CancellationToken ct);
}
