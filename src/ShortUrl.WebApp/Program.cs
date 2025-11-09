using Microsoft.OpenApi.Models;
using ShortUrl.Infrastructure;
using ShortUrl.WebApp.EndPoints;
using ShortUrl.WebApp.Integrations;
using ShortUrl.WebApp.Utility;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "My Short Url",
        Version = "v1",
        Description = "Simple project for a URL shortening"
    });

    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Description = "Insert your X-APIKEY",
        Type = SecuritySchemeType.ApiKey,
        Name = "X-APIKEY",
        In = ParameterLocation.Header,
        Scheme = "ApiKeyScheme"
    });

    c.OperationFilter<ApiKeyOperationFilter>();
});

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

// ScalaApi integration, only in Development environment
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRateLimiter();

app.UseStaticFiles();

app.MapDefaultEndpoints();

app.MapGet("/", () => "Simple ShortUrl Project")
    .ExcludeFromDescription();

app.MapShortUrlEndpoints()
    .MapManageShortUrlEndpoints()
    .MapQrCodeEndpoints();

await app.RunAsync();
