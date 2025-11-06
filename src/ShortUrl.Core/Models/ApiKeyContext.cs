namespace ShortUrl.Core.Models
{
    public class ApiKeyContext
    {
        public long Id { get; set; }
        public string ApiKey { get; set; } = string.Empty;
        public bool CanSetCustomShortCodes { get; set; }
    }
}
