using ShortUrl.Common;

namespace ShortUrl.Entities
{
    public class ApiKeyEntity
    {
        public long Id { get; private set; }

        public string ApiKey { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string Name { get; private set; } = string.Empty;
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        public bool CanSetCustomShortCodes { get; private set; }

        // Navigation properties
        public IEnumerable<ShortUrlEntity> ShortUrls { get; private set; } = [];

        private ApiKeyEntity() { }

        public static Result<ApiKeyEntity> Create(
            string apiKey,
            string email,
            string name,
            bool isActive,
            bool canSetCustomShortCodes
            )
        {
            // TODO validatori

            var entity = new ApiKeyEntity
            {
                ApiKey = apiKey,
                Email = email,
                Name = name,
                IsActive = isActive,
                CanSetCustomShortCodes = canSetCustomShortCodes
            };

            return entity;
        }
    }
}
