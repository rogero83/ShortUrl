namespace ShortUrl.Core.Models;

/// <summary>
/// Represents the result of a successful short URL creation operation.
/// </summary>
/// <param name="ShortCode">The unique short code generated for the newly created short URL. Cannot be null or empty.</param>
public record CreateShortUrlResponse(string ShortCode);
