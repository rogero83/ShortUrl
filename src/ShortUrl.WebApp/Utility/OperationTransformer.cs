using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace ShortUrl.WebApp.Utility;

public class OperationTransformer : IOpenApiOperationTransformer
{
    public Task TransformAsync(
        OpenApiOperation operation,
        OpenApiOperationTransformerContext context,
        CancellationToken cancellationToken)
    {
        var hasApiKeyAttribute = context.Description
                .ActionDescriptor
                .EndpointMetadata
                .OfType<RequireApiKeyMetaData>()
                .Any();

        if (!hasApiKeyAttribute)
            return Task.CompletedTask;

        operation.Security ??= new List<OpenApiSecurityRequirement>();

        operation.Security.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("ApiKey", context.Document)] = []
        });

        return Task.CompletedTask;
    }
}