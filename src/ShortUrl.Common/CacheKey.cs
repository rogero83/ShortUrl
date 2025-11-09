namespace ShortUrl.Common
{
    public static class CacheKey
    {
        public static string ApiKey(string key) => $"ak-{key}";
        public static string TagApiKey => "all-apikey";

        public static string ShortUrlKey(string shortUrl)
            => $"suk-{shortUrl}";

        public static string TagAllShortUrl
            => "tag-all-shorturl";
    }
}
