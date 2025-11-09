namespace ShortUrl.WebApp.Models;

/// <summary>
/// Respose model for testing shortCode
/// </summary>
/// <param name="LongUrl">Long url</param>
/// <param name="IpAddress">Request ip address</param>
/// <param name="UserAgent">Request User-agent</param>
/// <param name="Referrer">Request Referrer</param>
public record PreviewResponse(string LongUrl,
    string? IpAddress,
    string? UserAgent,
    string? Referrer);