using ShortUrl.Infrastructure.ApiKeyManager;

namespace ShortUrl.WebApp.Utility
{
    public class RequireApiKeyFilter(ILogger<RequireApiKeyFilter> logger,
        IApiKeyValidator validator,
        IApiKeyContextAccessor accessor)
        : IEndpointFilter
    {
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            var httpContext = context.HttpContext;

            if (!httpContext.Request.Headers.TryGetValue("x-apikey", out var apiKeyValue)
                || string.IsNullOrEmpty(apiKeyValue))
            {
                logger.LogWarning("Header x-apikey not found");
                return Results.Unauthorized();
            }

            var apiKey = await validator.Validate(apiKeyValue!);
            if (apiKey == null)
            {
                logger.LogWarning("Invalid API key attempted: {ApiKey}", apiKeyValue!);
                return Results.Unauthorized();
            }

            accessor.Current = apiKey!;

            return await next(context);
        }
    }

    /// <summary>
    /// Used for swagger doc
    /// </summary>
    public class RequireApiKeyMetaData { }
}
