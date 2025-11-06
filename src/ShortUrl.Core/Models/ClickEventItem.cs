using System.Diagnostics;

namespace ShortUrl.Core.Models;

public record ClickEventItem(Activity? Activity, long ShortUrlId, string? IpAddress, string? UserAgent, string? Referrer);

