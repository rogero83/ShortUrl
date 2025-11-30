namespace ShortUrl.Core.Models;

/// <summary>
/// Represents the context information associated with an API key, including its identifier, value, and permissions.
/// </summary>
public class ApiKeyContext
{
    /// <summary>
    /// Api key identifier.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Api key used to authenticate requests to external services.
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Indicating whether custom short codes can be set for this instance.
    /// </summary>
    public bool CanSetCustomShortCodes { get; set; }
}
