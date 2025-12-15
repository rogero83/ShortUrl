using Microsoft.OpenApi;
using Scalar.AspNetCore;
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
builder.Services.AddOpenApi("v1", options =>
{
    // Specify the OpenAPI version to use
    options.OpenApiVersion = OpenApiSpecVersion.OpenApi3_1;

    options.AddDocumentTransformer<DocumentTransformer>();

    options.AddOperationTransformer<OperationTransformer>();
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

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });
    app.MapScalarApiReference();
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
            var apiKeyEntity = ApiKeyEntity.Create(
                apiKeySeed,
                "test@test.com",
                "test",
                true,
                false);

            if (apiKeyEntity.IsFailure)
                throw new Exception(apiKeyEntity.Error.ToString());

            dbContext.ApiKeys.Add(apiKeyEntity.Value);
        }

        if (!dbContext.ApiKeys.Any(a => a.ApiKey == apiKeySeedCustomUrl))
        {
            var apiKeyentityCustomUrl = ApiKeyEntity.Create(
                apiKeySeedCustomUrl,
                "testcustom@test.com",
                "testCustomUrl",
                true,
                true);

            if (apiKeyentityCustomUrl.IsFailure)
                throw new Exception(apiKeyentityCustomUrl.Error.ToString());

            dbContext.ApiKeys.Add(apiKeyentityCustomUrl.Value);
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
