namespace ShortUrl.Entities
{
    public class ApiKeyEntity
    {
        public long Id { get; set; }

        public string ApiKey { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool CanSetCustomShortCodes { get; set; }

        // Navigation properties
        public IEnumerable<ShortUrlEntity> ShortUrls { get; set; } = [];
    }
}
