using ShortUrl.Core.Models;

namespace ShortUrl.Infrastructure.ApiKeyManager
{
    public interface IApiKeyContextAccessor
    {
        ApiKeyContext Current { get; set; }
    }

    public class ApiKeyContextAccessor : IApiKeyContextAccessor
    {
        public required ApiKeyContext Current { get; set; }
    }
}
