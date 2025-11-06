using ShortUrl.Aspire.Test.Fixture;
using ShortUrl.Core.Models;
using System.Net.Http.Json;

namespace ShortUrl.Aspire.Test
{
    [Collection("Startup collection")]
    public class CreateAndCall(StartupFixture fixture)
    {
        [Fact]
        public async Task CreateAndCall_Success()
        {
            var createRequest = new CreateShortUrlRequest("https://www.example.com/some/long/url");

            // Create short URL            
            using var createResponse = await fixture.ClientShortUrlApp(StartupFixture.ApiKeyLocal)
                .PostAsJsonAsync("/api/v1/create", createRequest, fixture.CancellationToken);

            Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);
            var shortUrl = await createResponse.Content.ReadFromJsonAsync<CreateShortUrlResponse>(fixture.CancellationToken);
            Assert.NotNull(shortUrl);

            // Call short URL
            using var callResponse = await fixture.ClientShortUrlApp()
                .GetAsync(shortUrl.ShortCode, fixture.CancellationToken);

            // Assert redirection
            Assert.Equal(HttpStatusCode.Redirect, callResponse.StatusCode);
            Assert.Equal(createRequest.OriginalUrl, callResponse.Headers.Location.ToString());
        }

        [Fact]
        public async Task CreateAndCall_WithExpired_Success()
        {
            var createRequest = new CreateShortUrlRequest(
                "https://www.example.com/some/long/url",
                Expire: DateTime.UtcNow.AddDays(10));

            // Create short URL            
            using var createResponse = await fixture.ClientShortUrlApp(StartupFixture.ApiKeyLocal)
                .PostAsJsonAsync("/api/v1/create", createRequest, fixture.CancellationToken);

            Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);
            var shortUrl = await createResponse.Content.ReadFromJsonAsync<CreateShortUrlResponse>(fixture.CancellationToken);
            Assert.NotNull(shortUrl);

            // Call short URL
            using var callResponse = await fixture.ClientShortUrlApp()
                .GetAsync(shortUrl.ShortCode, fixture.CancellationToken);

            // Assert redirection
            Assert.Equal(HttpStatusCode.Redirect, callResponse.StatusCode);
            Assert.Equal(createRequest.OriginalUrl, callResponse.Headers.Location.ToString());
        }

        [Fact]
        public async Task CreateAndCall_WithNoUrl_Error()
        {
            var createRequest = new CreateShortUrlRequest("www.example.com/some/long/url");

            // Create short URL            
            using var createResponse = await fixture.ClientShortUrlApp(StartupFixture.ApiKeyLocal)
                .PostAsJsonAsync("/api/v1/create", createRequest, fixture.CancellationToken);

            Assert.Equal(HttpStatusCode.BadRequest, createResponse.StatusCode);
        }

        [Fact]
        public async Task CreateAndCall_WithExpiredInPast_Error()
        {
            var createRequest = new CreateShortUrlRequest(
                "https://www.example.com/some/long/url",
                Expire: DateTime.UtcNow.AddDays(-1));

            // Create short URL            
            using var createResponse = await fixture.ClientShortUrlApp(StartupFixture.ApiKeyLocal)
                .PostAsJsonAsync("/api/v1/create", createRequest, fixture.CancellationToken);

            Assert.Equal(HttpStatusCode.BadRequest, createResponse.StatusCode);
        }

        [Fact]
        public async Task CreateAndCall_WithCustomUrlNotAllowed_Error()
        {
            var createRequest = new CreateShortUrlRequest("https://www.example.com/some/long/url")
            {
                ShortCode = "custom-not-allowed"
            };

            // Create short URL            
            using var createResponse = await fixture.ClientShortUrlApp(StartupFixture.ApiKeyLocal)
                .PostAsJsonAsync("/api/v1/create", createRequest, fixture.CancellationToken);

            Assert.Equal(HttpStatusCode.BadRequest, createResponse.StatusCode);
        }

        [Fact]
        public async Task CreateAndCall_CallAfterExpired_Success()
        {
            var client = fixture.ClientShortUrlApp(StartupFixture.ApiKeyLocal);

            var createRequest = new CreateShortUrlRequest(
                "https://www.example.com/some/long/url",
                Expire: DateTime.UtcNow.AddSeconds(3));

            // Create short URL            
            using var createResponse = await client
                .PostAsJsonAsync("/api/v1/create", createRequest, fixture.CancellationToken);

            Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);
            var shortUrl = await createResponse.Content.ReadFromJsonAsync<CreateShortUrlResponse>(fixture.CancellationToken);
            Assert.NotNull(shortUrl);

            // Wait until expired
            await Task.Delay(TimeSpan.FromSeconds(3), fixture.CancellationToken);

            // Call short URL
            using var callResponse = await fixture.ClientShortUrlApp()
                .GetAsync(shortUrl.ShortCode, fixture.CancellationToken);

            // Assert expired
            Assert.Equal(HttpStatusCode.NotFound, callResponse.StatusCode);
        }
    }
}
