using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ShortUrl.DbPersistence;
using ShortUrl.Entities;

namespace ShortUrl.DevSupport;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("ShortUrl.DevSupport starting...");
        if (args != null)
        {
            Console.WriteLine(string.Join(",", args));
        }

        var builder = Host.CreateApplicationBuilder();
        builder.AddServiceDefaults();

        builder.AddShortUrlDbPersistenceIntegration();

        var app = builder.Build();

        // Ottieni il service provider per le migration EF
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ShortUrlContext>();

        if (args != null && args.Length > 0)
        {
            if (args.Contains("migrate"))
            {
                Console.WriteLine("EnsureCreated");
                dbContext.Database.Migrate();
            }

            if (args.Contains("seed"))
            {
                Console.WriteLine("Seed");

                var apiKeySeed = "api-key-local";
                var apiKeySeedCustomUrl = "api-key-local-custom-url";

                if (!dbContext.ApiKeys.Any(a => a.ApiKey == apiKeySeed))
                {
                    var apiKeyentity = new ApiKeyEntity
                    {
                        ApiKey = apiKeySeed,
                        Email = "test@test.com",
                        Name = "test",
                        IsActive = true,
                        CanSetCustomShortCodes = false
                    };
                    dbContext.ApiKeys.Add(apiKeyentity);
                }

                if (!dbContext.ApiKeys.Any(a => a.ApiKey == apiKeySeedCustomUrl))
                {
                    var apiKeyentityCustomUrl = new ApiKeyEntity
                    {
                        ApiKey = apiKeySeedCustomUrl,
                        Email = "testcustom@test.com",
                        Name = "testCustomUrl",
                        IsActive = true,
                        CanSetCustomShortCodes = true
                    };
                    dbContext.ApiKeys.Add(apiKeyentityCustomUrl);
                }

                dbContext.SaveChanges();

            }
        }
        else
        {
            Console.WriteLine("No args");
        }


        Console.WriteLine("Bye Bye");
    }
}
