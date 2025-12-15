using ShortUrl.Common;

namespace ShortUrl.Entities
{
    public class ShortUrlEntity
    {
        public long Id { get; private set; }

        public string ShortCode { get; private set; } = string.Empty;
        public string LongUrl { get; private set; } = string.Empty;

        public long OwnerId { get; private set; }

        public long TotalClicks { get; private set; } = 0;
        public long UniqueClicks { get; private set; } = 0;

        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime? ExpiresAt { get; private set; } = null;

        // Navigation properties
        public ClickEventEntity[] ClickEvents { get; private set; } = [];
        public ApiKeyEntity Owner { get; private set; } = null!;

        private ShortUrlEntity() { }

        public static Result<ShortUrlEntity> Create(
            string shortCode,
            long ownerId,
            string originalUrl,
            DateTime? expire,
            bool isActive
            )
        {
            if (string.IsNullOrEmpty(originalUrl)
                || !Uri.IsWellFormedUriString(originalUrl, UriKind.Absolute))
            {
                return Error.Validation("The provided long URL is not valid.");
            }

            if (expire.HasValue && expire.Value <= DateTime.UtcNow)
            {
                return Error.Failure("Expire date error");
            }

            var entity = new ShortUrlEntity
            {
                ShortCode = shortCode,
                OwnerId = ownerId,
                LongUrl = originalUrl,
                ExpiresAt = expire,
                IsActive = isActive
            };

            return entity;
        }

        public Result Edit(string originalUrl, DateTime? expire, bool isActive)
        {
            if (string.IsNullOrEmpty(originalUrl)
                || !Uri.IsWellFormedUriString(originalUrl, UriKind.Absolute))
            {
                return Error.Validation("The provided long URL is not valid.");
            }

            if (expire.HasValue && expire.Value <= DateTime.UtcNow)
            {
                return Error.Validation("The expiration date must be in the future.");
            }

            LongUrl = originalUrl;
            IsActive = isActive;
            ExpiresAt = expire;

            return Result.Success();
        }
    }
}
