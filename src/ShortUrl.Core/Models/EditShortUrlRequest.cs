namespace ShortUrl.Core.Models;

/// <summary>
/// Represents a request to update the properties of an existing short URL, including its destination, activation
/// status, and optional expiration date.
/// </summary>
/// <param name="OriginalUrl">The new destination URL to which the short URL should redirect. This value cannot be null or empty.</param>
/// <param name="IsActive">A value indicating whether the short URL should be active. Set to <see langword="true"/> to enable the short URL;
/// otherwise, <see langword="false"/>.</param>
/// <param name="Expire">The optional expiration date and time for the short URL. If specified, the short URL will become inactive after this
/// date. If null, the short URL does not expire.</param>
public record EditShortUrlRequest(string OriginalUrl, bool IsActive = true, DateTime? Expire = null);
