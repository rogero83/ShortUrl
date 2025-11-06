using ShortUrl.Aspire.Test.Fixture;

namespace ShortUrl.Aspire.Test
{
    [Collection("Startup collection")]
    public class ApiKeyTest(StartupFixture fixture)
    {
        [Fact]
        public async Task ApiPing_Success()
        {
            using var response = await fixture.ClientShortUrlApp(StartupFixture.ApiKeyLocal)
                .GetAsync("/api/v1/ping", fixture.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task ApiPing_ErrorKey()
        {
            using var response = await fixture.ClientShortUrlApp("wrong-api-key-local")
                .GetAsync("/api/v1/ping", fixture.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task ApiPing_NoKey()
        {
            using var response = await fixture.ClientShortUrlApp()
                .GetAsync("/api/v1/ping", fixture.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
