namespace ShortUrl.Core.Models;

/// <summary>
/// Represents a paginated response containing a collection of short URL information and pagination details.
/// </summary>
public class ShortCodesResponse
{
    /// <summary>
    /// Total number of items available across all pages.
    /// </summary>
    public int TotalItems { get; set; }
    /// <summary>
    /// Actual page number of the current set of results.
    /// </summary>
    public int Page { get; set; }
    /// <summary>
    /// Items included per page in the response.
    /// </summary>
    public int ItemsByPage { get; set; }
    /// <summary>
    /// Results containing the list of short URL information.
    /// </summary>
    public List<ShortUrlInfo> Items { get; set; } = new();
}

/// <summary>
/// Short URL information details.
/// </summary>
public record ShortUrlInfo
{
    /// <summary>
    /// Short code representing the shortened URL.
    /// </summary>
    public string ShortCode { get; set; } = string.Empty;
    /// <summary>
    /// Original long URL before shortening.
    /// </summary>
    public string LongUrl { get; set; } = string.Empty;
    /// <summary>
    /// Indicates whether the short URL is currently active.
    /// </summary>
    public bool IsActive { get; set; }
    /// <summary>
    /// Total number of clicks the short URL has received.
    /// </summary>
    public long TotalClicks { get; set; } = 0;
    /// <summary>
    /// Total number of unique clicks the short URL has received.
    /// </summary>
    public long UniqueClicks { get; set; } = 0;
    /// <summary>
    /// Creation date and time of the short URL.
    /// </summary>
    public DateTime CreatedAt { get; set; }
    /// <summary>
    /// Expiration date and time of the short URL, if set; otherwise, null.
    /// </summary>
    public DateTime? ExpireAt { get; set; }
    /// <summary>
    /// Clicks in the last month.
    /// </summary>
    public int ClicksInLastMonth { get; set; }
}
