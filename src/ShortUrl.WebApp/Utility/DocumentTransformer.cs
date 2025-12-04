using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace ShortUrl.WebApp.Utility;

public class DocumentTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        document.Info = new()
        {
            Title = "My Short Url",
            Version = "v1",
            Description = "Simple project for a URL shortening"
        };

        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes = new Dictionary<string, IOpenApiSecurityScheme>
        {
            {
                "ApiKey",
                new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.ApiKey,
                    Name = "X-APIKEY",
                    In = ParameterLocation.Header,
                    Description = "API Key per l'autenticazione"
                }
            }
        };

        return Task.CompletedTask;
    }
}
