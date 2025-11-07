using Microsoft.AspNetCore.Mvc;
using QRCoder;
using ShortUrl.Core.Contracts;
using ShortUrl.WebApp.Utility;

namespace ShortUrl.WebApp.EndPoints;

public static class QrCodeEndpoints
{
    public static WebApplication MapQrCodeEndpoints(this WebApplication app)
    {
        const int size = 10;

        app.MapGet("/qr/{shortCode}", async (
            string shortCode,
            HttpContext context,
            IReadShortUrlService svc,
            CancellationToken ct,
            [FromQuery] string format = "png") =>
        {
            var exists = await svc.Exists(shortCode, ct);
            if (!exists)
            {
                return Results.NotFound("Short code does not exist.");
            }

            var outputFormat = !string.IsNullOrWhiteSpace(format) ? format.ToLowerInvariant() : "png";

            var url = $"{context.Request.Scheme}://{context.Request.Host}/{shortCode}";

            using var qrGenerator = new QRCodeGenerator();
            var payload = new PayloadGenerator.Url(url);
            using var qrCodeData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);

            switch (outputFormat)
            {
                case "svg":
                    {
                        using var qrCode = new SvgQRCode(qrCodeData);
                        var svgImage = qrCode.GetGraphic(size);
                        return Results.Content(svgImage, "image/svg+xml");
                    }
                case "png":
                    {
                        using var qrCode = new PngByteQRCode(qrCodeData);
                        var pngImage = qrCode.GetGraphic(size);
                        return Results.File(pngImage, "image/png");
                    }
                default:
                    return Results.BadRequest("Unsupported format. Please use 'png' or 'svg'.");
            }
        }).RequireRateLimiting(RateLimiterUtility.BaseFixed);

        return app;
    }
}