using ShortUrl.Core.Contracts;
using ShortUrl.Core.Models;
using ShortUrl.WebApp.Models;
using ShortUrl.WebApp.Utility;
using System.Diagnostics;

namespace ShortUrl.WebApp.EndPoints
{
    public static class ShortUrlEndpoints
    {
        public static WebApplication MapShortUrlEndpoints(this WebApplication app)
        {
            app.MapGet("/{shortUrl}", async (string shortUrl,
                IReadShortUrlService svc,
                IClickEventChannel channel,
                IHttpContextAccessor httpContextAccessor,
                CancellationToken ct) =>
            {
                var currentActivity = Activity.Current;

                var result = await svc.GetLongUrl(shortUrl, CancellationToken.None);
                if (result.IsSuccess)
                {
                    var url = result.Value;
                    var context = httpContextAccessor.HttpContext;

                    _ = channel.WriteAsync(new ClickEventItem(
                        currentActivity,
                        url.Id,
                        // Get additional info from HttpContext
                        context?.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                            ?? context?.Connection.RemoteIpAddress?.ToString(),
                        context?.Request.Headers.UserAgent.ToString(),
                        context?.Request.Headers.Referer.ToString(),
                        DateTime.UtcNow
                    ), ct);

                    return Results.Redirect(url.LognUrl);
                }
                else
                {
                    return Results.NotFound(result.Error.Message);
                }
            }).RequireRateLimiting(RateLimiterUtility.BaseFixed)
            .WithOpenApi()
            .WithTags(ApiTags.Discover)
            .WithDescription("")
            .Produces(StatusCodes.Status302Found)
            .Produces(StatusCodes.Status404NotFound);

            app.MapGet("/preview/{shortUrl}", async (string shortUrl,
                IReadShortUrlService svc,
                IHttpContextAccessor httpContextAccessor,
                CancellationToken ct) =>
            {
                var currentActivity = Activity.Current;

                var result = await svc.GetLongUrl(shortUrl, CancellationToken.None);
                if (result.IsSuccess)
                {
                    var url = result.Value;
                    var context = httpContextAccessor.HttpContext;

                    return Results.Ok(new PreviewResponse(
                        url.LognUrl,
                        context?.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                            ?? context?.Connection.RemoteIpAddress?.ToString(),
                        context?.Request.Headers.UserAgent.ToString(),
                        context?.Request.Headers.Referer.ToString()));
                }
                else
                {
                    return Results.NotFound(result.Error.Message);
                }
            }).RequireRateLimiting(RateLimiterUtility.BaseFixed)
            .WithTags(ApiTags.Discover)
            .WithDescription("Testing url")
            .Produces<PreviewResponse>()
            .Produces<string>(StatusCodes.Status404NotFound);

            return app;
        }
    }
}
