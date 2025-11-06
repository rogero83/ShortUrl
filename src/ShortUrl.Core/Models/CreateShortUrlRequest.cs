namespace ShortUrl.Core.Models
{
    public record CreateShortUrlRequest(string OriginalUrl,
        bool Activate = true,
        DateTime? Expire = null)
    {
        /// <summary>
        /// Only for apikeys with permission to set custom short codes.
        /// </summary>
        public string? ShortCode { get; set; }
    };
}
