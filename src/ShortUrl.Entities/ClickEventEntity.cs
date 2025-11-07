namespace ShortUrl.Entities
{
    public class ClickEventEntity
    {
        public long Id { get; set; }
        public long ShortUrlId { get; set; }

        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public string? Referrer { get; set; }

        public DateTime ClickedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ShortUrlEntity ShortUrl { get; set; } = null!;
    }
}
