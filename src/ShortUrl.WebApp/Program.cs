using Microsoft.OpenApi.Models;
using ShortUrl.DbPersistence;
using ShortUrl.Entities;
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

#region initData for Development
if (!app.Environment.IsProduction())
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<ShortUrlContext>();

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

        await dbContext.SaveChangesAsync();

    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Errore durante il seed del database");
    }
}
#endregion initData for Development

await app.RunAsync();
