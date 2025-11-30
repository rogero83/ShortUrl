namespace ShortUrl.Core.Models;

/// <summary>
/// Represents the context information associated with an API key, including its identifier, value, and permissions.
/// </summary>
public class ApiKeyContext
{
    public long Id { get; set; }
    public string ApiKey { get; set; } = string.Empty;
    public bool CanSetCustomShortCodes { get; set; }
}
