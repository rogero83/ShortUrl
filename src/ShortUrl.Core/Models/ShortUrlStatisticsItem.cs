namespace ShortUrl.Core.Models;

/// <summary>
/// Represents statistical information for a shortened URL, including its short code, original URL, and the number of
/// times it has been accessed.
/// </summary>
public class ShortUrlStatisticsItem
{
    public string? ShortCode { get; set; }
    public string? OriginalUrl { get; set; }
    public long ClickCount { get; set; }
}
