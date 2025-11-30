namespace ShortUrl.Core.Models;

/// <summary>
/// Represents a search result item containing information about a shortened URL.
/// </summary>
/// <param name="Id">The unique identifier of the shortened URL.</param>
/// <param name="LognUrl">The original, unshortened URL associated with the shortened link. Cannot be null.</param>
public record ShortUrlSearchItem(long Id, string LognUrl);

