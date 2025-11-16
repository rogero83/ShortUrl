using Aspire.Hosting;
using Microsoft.Extensions.Logging;

namespace ShortUrl.Aspire.Test.Fixture
{
    public class StartupFixture : IAsyncLifetime
    {
        public static string ApiKeyLocal => "api-key-local";
        public static string ApiKeyLocalCustomUrl => "api-key-local-custom-url";

        private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(60);

        private IDictionary<string, HttpClient> ClientsShortUrl { get; set; } = new Dictionary<string, HttpClient>();
        public CancellationToken CancellationToken => TestContext.Current.CancellationToken;
#pragma warning disable CS8618
        private DistributedApplication App { get; set; }

        public async ValueTask InitializeAsync()
        {
            var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.ShortUrl_AppHost>(/*["test"],*/ CancellationToken);
            appHost.Services.AddLogging(logging =>
            {
                logging.SetMinimumLevel(LogLevel.Debug);
                // Override the logging filters from the app's configuration
                logging.AddFilter(appHost.Environment.ApplicationName, LogLevel.Debug);
                logging.AddFilter("Aspire.", LogLevel.Debug);
                // To output logs to the xUnit.net ITestOutputHelper, consider adding a package from https://www.nuget.org/packages?q=xunit+logging
            });
            appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
            {
                clientBuilder.AddStandardResilienceHandler();
            });

            App = await appHost.BuildAsync(CancellationToken).WaitAsync(DefaultTimeout, CancellationToken);
            await App.StartAsync(CancellationToken).WaitAsync(DefaultTimeout, CancellationToken);

            await App.ResourceNotifications.WaitForResourceHealthyAsync("shorturl-webapp", CancellationToken).WaitAsync(DefaultTimeout, CancellationToken);
        }

        public HttpClient ClientShortUrlApp(string? apiKey = null)
        {
            if (ClientsShortUrl.TryGetValue(apiKey ?? string.Empty, out var client))
            {
                return client;
            }

            var endpoint = App.GetEndpoint("shorturl-webapp");
            var handler = new HttpClientHandler { AllowAutoRedirect = false };
            var httpClient = new HttpClient(handler) { BaseAddress = endpoint };
            if (!string.IsNullOrEmpty(apiKey))
            {
                httpClient.DefaultRequestHeaders.Add("x-apikey", apiKey);
            }
            ClientsShortUrl[apiKey ?? string.Empty] = httpClient;

            return httpClient;
        }

        public async ValueTask DisposeAsync()
        {
            foreach (var ClientShortUrlApp in ClientsShortUrl.Values)
                ClientShortUrlApp.Dispose();

            await App.DisposeAsync();
        }
    }

    [CollectionDefinition("Startup collection")]
    public class StartupCollection : ICollectionFixture<StartupFixture>
    {
        // vuota — serve solo per dichiarare la collection
    }
}
