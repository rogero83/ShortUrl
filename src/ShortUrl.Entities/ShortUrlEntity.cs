namespace ShortUrl.Entities
{
    public class ShortUrlEntity
    {
        public long Id { get; set; }

        public string ShortCode { get; set; } = string.Empty;
        public string LongUrl { get; set; } = string.Empty;

        public long OwnerId { get; set; }

        public long TotalClicks { get; set; } = 0;
        public long UniqueClicks { get; set; } = 0;

        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ExpiresAt { get; set; } = null;

        // Navigation properties
        public ClickEventEntity[] ClickEvents { get; set; } = [];
        public ApiKeyEntity Owner { get; set; } = null!;
    }
}
