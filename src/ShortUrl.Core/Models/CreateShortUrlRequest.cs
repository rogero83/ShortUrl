namespace ShortUrl.Core.Models
{
    /// <summary>
    /// Represents a request to create a new shortened URL with optional activation and expiration settings.
    /// </summary>
    /// <param name="OriginalUrl">The original URL to be shortened. This value must be a valid absolute URL.</param>
    /// <param name="Activate">A value indicating whether the shortened URL should be active upon creation. Defaults to <see langword="true"/>.</param>
    /// <param name="Expire">The optional expiration date and time for the shortened URL. If <see langword="null"/>, the URL does not expire.</param>
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
