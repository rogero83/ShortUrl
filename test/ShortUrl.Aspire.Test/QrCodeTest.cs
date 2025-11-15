using ShortUrl.Aspire.Test.Fixture;
using ShortUrl.Core.Models;
using System.Net.Http.Json;

namespace ShortUrl.Aspire.Test;

[Collection("Startup collection")]
public class QrCodeTest(StartupFixture fixture)
{
    private async Task<string> GetValidShortCode()
    {
        var createRequest = new CreateShortUrlRequest($"https://www.example.com/some/long/url/{Guid.NewGuid()}");
        using var createResponse = await fixture.ClientShortUrlApp(StartupFixture.ApiKeyLocal)
            .PostAsJsonAsync("/api/v1/create", createRequest, fixture.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);
        var shortUrl = await createResponse.Content.ReadFromJsonAsync<CreateShortUrlResponse>(fixture.CancellationToken);
        Assert.NotNull(shortUrl);
        return shortUrl.ShortCode;
    }

    [Fact]
    public async Task GetQrCode_ForExistingUrl_ReturnsPng()
    {
        // Arrange
        var shortCode = await GetValidShortCode();

        // Act
        using var response = await fixture.ClientShortUrlApp().GetAsync($"/qr/{shortCode}", fixture.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("image/png", response.Content.Headers.ContentType?.MediaType);
        var imageBytes = await response.Content.ReadAsByteArrayAsync(fixture.CancellationToken);
        Assert.NotEmpty(imageBytes);
    }

    [Fact]
    public async Task GetQrCode_ForExistingUrl_ReturnsSvg()
    {
        // Arrange
        var shortCode = await GetValidShortCode();

        // Act
        using var response = await fixture.ClientShortUrlApp().GetAsync($"/qr/{shortCode}?format=svg", fixture.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("image/svg+xml", response.Content.Headers.ContentType?.MediaType);
        var svgString = await response.Content.ReadAsStringAsync(fixture.CancellationToken);
        Assert.StartsWith("<svg", svgString);
        Assert.EndsWith("</svg>", svgString);
    }

    [Fact]
    public async Task GetQrCode_ForNonExistentUrl_ReturnsNotFound()
    {
        // Arrange
        var nonExistentCode = $"non-existent-{Guid.NewGuid()}";

        // Act
        using var response = await fixture.ClientShortUrlApp().GetAsync($"/qr/{nonExistentCode}", fixture.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetQrCode_WithUnsupportedFormat_ReturnsBadRequest()
    {
        // Arrange
        var shortCode = await GetValidShortCode();

        // Act
        using var response = await fixture.ClientShortUrlApp().GetAsync($"/qr/{shortCode}?format=jpeg", fixture.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
