using ShortUrl.Aspire.Test.Fixture;
using ShortUrl.Common;
using ShortUrl.Core.Models;
using System.Net.Http.Json;

namespace ShortUrl.Aspire.Test;

[Collection("Startup collection")]
public class CreateAndCallCustomUrl(StartupFixture fixture)
{
    [Fact]
    public async Task CreateAndCallCustomUrl_Success()
    {
        var customShortCode = $"custom-{ShortUrlGenerator.GenerateShortCode()}";
        var createRequest = new CreateShortUrlRequest("https://www.example.com/some/long/url")
        {
            ShortCode = customShortCode
        };

        // Create short URL            
        using var createResponse = await fixture.ClientShortUrlApp(StartupFixture.ApiKeyLocalCustomUrl)
            .PostAsJsonAsync("/api/v1/create", createRequest, fixture.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);
        var shortUrl = await createResponse.Content.ReadFromJsonAsync<CreateShortUrlResponse>(fixture.CancellationToken);
        Assert.NotNull(shortUrl);
        Assert.Equal(customShortCode, shortUrl.ShortCode);

        // Call short URL
        using var callResponse = await fixture.ClientShortUrlApp()
            .GetAsync(customShortCode, fixture.CancellationToken);

        // Assert redirection
        Assert.Equal(HttpStatusCode.Redirect, callResponse.StatusCode);
        Assert.Equal(createRequest.OriginalUrl, callResponse.Headers.Location!.ToString());
    }

    [Fact]
    public async Task CreateAndCall_WithExpired_Success()
    {
        var customShortCode = $"custom-{ShortUrlGenerator.GenerateShortCode()}";
        var createRequest = new CreateShortUrlRequest(
            "https://www.example.com/some/long/url",
            Expire: DateTime.UtcNow.AddDays(10))
        {
            ShortCode = customShortCode
        };

        // Create short URL            
        using var createResponse = await fixture.ClientShortUrlApp(StartupFixture.ApiKeyLocalCustomUrl)
            .PostAsJsonAsync("/api/v1/create", createRequest, fixture.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);
        var shortUrl = await createResponse.Content.ReadFromJsonAsync<CreateShortUrlResponse>(fixture.CancellationToken);
        Assert.NotNull(shortUrl);
        Assert.Equal(customShortCode, shortUrl.ShortCode);

        // Call short URL
        using var callResponse = await fixture.ClientShortUrlApp()
            .GetAsync(customShortCode, fixture.CancellationToken);

        // Assert redirection
        Assert.Equal(HttpStatusCode.Redirect, callResponse.StatusCode);
        Assert.Equal(createRequest.OriginalUrl, callResponse.Headers.Location!.ToString());
    }

    [Theory]
    [InlineData(3)] // Too short
    [InlineData(201)] // Too long
    public async Task CreateAndCall_CustomCodeValidationLenth_Error(int codeLenth)
    {
        var createRequest = new CreateShortUrlRequest("https://www.example.com/some/long/url")
        {
            ShortCode = ShortUrlGenerator.GenerateShortCode(codeLenth)
        };

        // Create short URL            
        using var createResponse = await fixture.ClientShortUrlApp(StartupFixture.ApiKeyLocalCustomUrl)
            .PostAsJsonAsync("/api/v1/create", createRequest, fixture.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, createResponse.StatusCode);
    }

    [Theory]
    [InlineData("code!")]
    [InlineData("code=")]
    [InlineData("code/code")]
    [InlineData("code+code")]
    [InlineData("123#abc")]
    [InlineData("")]
    [InlineData(" ")]
    public async Task CreateAndCall_CustomCodeValidationAlphabet_Error(string shortCode)
    {
        var createRequest = new CreateShortUrlRequest("https://www.example.com/some/long/url")
        {
            ShortCode = shortCode
        };

        // Create short URL            
        using var createResponse = await fixture.ClientShortUrlApp(StartupFixture.ApiKeyLocalCustomUrl)
            .PostAsJsonAsync("/api/v1/create", createRequest, fixture.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, createResponse.StatusCode);
    }
}