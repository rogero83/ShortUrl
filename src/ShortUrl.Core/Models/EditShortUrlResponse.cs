namespace ShortUrl.Core.Models;

/// <summary>
/// Represents the result of an operation to edit an existing short URL.
/// </summary>
/// <param name="ShortCode">The unique code identifying the edited short URL. Cannot be null or empty.</param>
public record EditShortUrlResponse(string ShortCode);
