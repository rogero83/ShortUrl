using ShortUrl.Aspire.Test.Fixture;
using ShortUrl.Core.Models;
using System.Net.Http.Json;

namespace ShortUrl.Aspire.Test;

[Collection("Startup collection")]
public class CreateAndEdit(StartupFixture fixture)
{
    [Fact]
    public async Task CreateAndEdit_Success()
    {
        var createRequest = new CreateShortUrlRequest("https://www.example.com/some/long/url");

        var client = fixture.ClientShortUrlApp(StartupFixture.ApiKeyLocal);

        // Create short URL            
        var createResponse = await client
            .PostAsJsonAsync("/api/v1/create", createRequest, fixture.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);
        var shortUrl = await createResponse.Content.ReadFromJsonAsync<CreateShortUrlResponse>(fixture.CancellationToken);
        Assert.NotNull(shortUrl);

        // Edit short URL
        var editRequest = new EditShortUrlRequest("https://www.example.com/some/other/long/url");

        var editResponse = await client
            .PutAsJsonAsync($"/api/v1/edit/{shortUrl.ShortCode}", editRequest, fixture.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, editResponse.StatusCode);
        var shortUrlEdited = await editResponse.Content.ReadFromJsonAsync<EditShortUrlResponse>(fixture.CancellationToken);
        Assert.NotNull(shortUrlEdited);
        Assert.Equal(shortUrl.ShortCode, shortUrlEdited.ShortCode);

        // Call short URL
        var callResponse = await fixture.ClientShortUrlApp()
            .GetAsync(shortUrl.ShortCode, fixture.CancellationToken);

        // Assert redirection
        Assert.Equal(HttpStatusCode.Redirect, callResponse.StatusCode);
        Assert.Equal(editRequest.OriginalUrl, callResponse.Headers.Location.ToString());
    }
}