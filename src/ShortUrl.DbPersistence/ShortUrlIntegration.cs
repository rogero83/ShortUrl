using Microsoft.Extensions.Hosting;

namespace ShortUrl.DbPersistence
{
    public static class ShortUrlIntegration
    {
        public static IHostApplicationBuilder AddShortUrlDbPersistenceIntegration(this IHostApplicationBuilder builder)
        {
            builder.AddNpgsqlDbContext<ShortUrlContext>("short-url-db");

            return builder;
        }
    }
}
