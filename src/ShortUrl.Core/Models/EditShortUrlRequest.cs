namespace ShortUrl.Core.Models
{
    public record EditShortUrlRequest(string OriginalUrl, bool IsActive = true, DateTime? Expire = null);
}
