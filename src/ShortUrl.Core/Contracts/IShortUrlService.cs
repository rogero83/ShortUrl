using ShortUrl.Common.ResultPattern;
using ShortUrl.Core.Models;

namespace ShortUrl.Core.Contracts
{
    public interface IShortUrlService
    {
        Task<Result<CreateShortUrlResponse>> CreateShortUrl(ApiKeyContext apiKeyContext, CreateShortUrlRequest request, CancellationToken ct);

        Task<Result<EditShortUrlResponse>> EditShortUrl(ApiKeyContext apiKeyContext, string shortCode, EditShortUrlRequest request, CancellationToken ct);
    }
}
