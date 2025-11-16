using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ShortUrl.Core.Contracts;
using ShortUrl.Core.Models;
using ShortUrl.Infrastructure.ApiKeyManager;
using ShortUrl.WebApp.Utility;
using System.ComponentModel.DataAnnotations;

namespace ShortUrl.WebApp.EndPoints
{
    public static class ManageShortUrlEndpoints
    {
        public static WebApplication MapManageShortUrlEndpoints(this WebApplication app)
        {
            var appGroup = app.MapGroup("/api/v1/")
                .AddEndpointFilter<RequireApiKeyFilter>()
                .RequireRateLimiting(RateLimiterUtility.BaseFixed)
                .WithTags(ApiTags.Api)
                .WithMetadata(new RequireApiKeyMetaData());

            appGroup.MapGet("ping", (
                IApiKeyContextAccessor apiKeyContext,
                CancellationToken ct) =>
            {
                return Results.Ok(new
                {
                    Message = "Pong",
                    ApiKey = apiKeyContext.Current
                });
            }).WithMetadata(new RequireApiKeyMetaData());

            appGroup.MapPost("create", async (IApiKeyContextAccessor apiKeyAccessor,
                CreateShortUrlRequest request, [FromServices] IValidator<CreateShortUrlRequest> validator, IShortUrlService svc, CancellationToken ct) =>
            {
                var validationResult = await validator.ValidateAsync(request, ct);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors
                        .Select(e => new ValidationResult(e.ErrorMessage, new[] { e.PropertyName }))
                        .ToList();
                    return Results.BadRequest(errors.ToDictionary(
                        e => e.MemberNames.FirstOrDefault() ?? string.Empty,
                        e => new[] { e.ErrorMessage }));
                }

                var result = await svc.CreateShortUrl(apiKeyAccessor.Current, request, ct);

                return result.ToResponse();
            });

            appGroup.MapPut("edit/{shortCode}", async (string shortCode,
                IApiKeyContextAccessor apiKeyAccessor,
                EditShortUrlRequest request, IShortUrlService svc, CancellationToken ct) =>
            {
                var result = await svc.EditShortUrl(apiKeyAccessor.Current, shortCode, request, ct);

                return result.ToResponse();
            });

            appGroup.MapPost("lists", async (ShortCodesRequest request,
                IApiKeyContextAccessor apiKeyAccessor,
                IShortUrlService svc, CancellationToken ct) =>
            {
                // Validate request
                // TODO: Add validator if needed

                var result = await svc.ListShortUrls(apiKeyAccessor.Current, request, ct);

                return result.ToResponse();
            });

            return app;
        }
    }
}
