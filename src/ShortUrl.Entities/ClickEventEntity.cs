namespace ShortUrl.Entities
{
    public class ClickEventEntity
    {
        public long Id { get; set; }
        public long ShortUrlId { get; set; }

        public string IpAddress { get; set; } = string.Empty;
        public string UserAgent { get; set; } = string.Empty;
        public string Referrer { get; set; } = string.Empty;

        public DateTime ClickedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ShortUrlEntity ShortUrl { get; set; } = null!;
    }
}
