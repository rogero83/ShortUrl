using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ShortUrl.WebApp.Utility
{
    public class ApiKeyOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var hasApiKeyAttribute = context.ApiDescription
                .ActionDescriptor
                .EndpointMetadata
                .OfType<RequireApiKeyMetaData>()
                .Any();

            if (!hasApiKeyAttribute)
                return;

            operation.Security = new List<OpenApiSecurityRequirement>
            {
                new()
                {
                    [new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "ApiKey"
                        }
                    }] = new List<string>()
                }
            };
        }
    }
}
