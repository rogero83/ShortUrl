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

        public static string TagShortUrlApiKey(long apiKeyId)
            => $"tag-apikey-{apiKeyId}";

        public static string TotalShortUrls => "total-short-urls";
        public static string TotalClicks => "total-clicks";
        public static string TopShortUrls(int count) => $"top-short-urls-{count}";
        public static string RecentClicks(int count) => $"recent-clicks-{count}";
    }
}
