using ShortUrl.Common;

namespace ShortUrl.Entities
{
    public class ClickEventEntity
    {
        public long Id { get; private set; }
        public long ShortUrlId { get; private set; }

        public string? IpAddress { get; private set; }
        public string? UserAgent { get; private set; }
        public string? Referrer { get; private set; }

        public DateTime ClickedAt { get; private set; } = DateTime.UtcNow;

        // Navigation properties
        public ShortUrlEntity ShortUrl { get; private set; } = null!;

        private ClickEventEntity() { }

        public static Result<ClickEventEntity> Create(
            long shortUrlId,
            string? ipAddress,
            string? userAgent,
            string? referrer,
            DateTime clickedAt)
        {
            var entity = new ClickEventEntity
            {
                ShortUrlId = shortUrlId,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                Referrer = referrer,
                ClickedAt = clickedAt
            };

            return entity;
        }
    }
}
