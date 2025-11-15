namespace ShortUrl.Core.Models;

public class ShortCodesResponse
{
    public int TotalItems { get; set; }
    public int Page { get; set; }
    public int ItemsByPage { get; set; }
    public List<ShortUrlInfo> Items { get; set; } = new();
}

public record ShortUrlInfo
{
    public string ShortCode { get; set; } = string.Empty;
    public string LongUrl { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public long TotalClicks { get; set; } = 0;
    public long UniqueClicks { get; set; } = 0;
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpireAt { get; set; }
    public int Clicks { get; set; }
}
