namespace ShortUrl.WebApp.Models;

public record PreviewResponse(string LongUrl,
    string? IpAddress,
    string? UserAgent,
    string? Referrer);