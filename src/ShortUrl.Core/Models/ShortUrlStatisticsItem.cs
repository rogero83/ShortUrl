namespace ShortUrl.Core.Models
{
    public class ShortUrlStatisticsItem
    {
        public string? ShortCode { get; set; }
        public string? OriginalUrl { get; set; }
        public long ClickCount { get; set; }
    }
}
