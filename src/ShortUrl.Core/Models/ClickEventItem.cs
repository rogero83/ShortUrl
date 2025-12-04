using System.Diagnostics;

namespace ShortUrl.Core.Models;

/// <summary>
/// Represents a record of a single click event associated with a shortened URL, including contextual information about
/// the click.
/// </summary>
/// <param name="Activity">The activity associated with the click event, or null if no activity is linked.</param>
/// <param name="ShortUrlId">The unique identifier of the shortened URL that was clicked.</param>
/// <param name="IpAddress">The IP address from which the click originated, or null if unavailable.</param>
/// <param name="UserAgent">The user agent string of the client that performed the click, or null if not provided.</param>
/// <param name="Referrer">The referring URL, if available, that led to the click event; otherwise, null.</param>
/// <param name="ClickedAt">The date and time, in UTC, when the click occurred.</param>
public record ClickEventItem(Activity? Activity,
    long ShortUrlId,
    string? IpAddress,
    string? UserAgent,
    string? Referrer,
    DateTime ClickedAt);

