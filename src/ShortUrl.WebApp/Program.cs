using ShortUrl.Infrastructure;
using ShortUrl.WebApp.EndPoints;
using ShortUrl.WebApp.Integrations;
using ShortUrl.WebApp.Utility;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

builder.AddInfrastructureService();
builder.AddIntegrationsWebService(builder.Configuration);

// Simple rate limiting policy in memory
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy(RateLimiterUtility.BaseFixed, httpContext => RateLimitPartition.GetFixedWindowLimiter(
        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
        factory: _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 100,
            Window = TimeSpan.FromSeconds(10)
        }));
});

var app = builder.Build();

app.UseRateLimiter();

app.UseStaticFiles();

app.MapDefaultEndpoints();

app.MapGet("/", () => "Simple ShortUrl Project");

app.MapShortUrlEndpoints()
    .MapManageShortUrlEndpoints()
    .MapQrCodeEndpoints();

await app.RunAsync();
