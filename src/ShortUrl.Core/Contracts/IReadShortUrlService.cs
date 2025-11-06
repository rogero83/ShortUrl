using ShortUrl.Common.ResultPattern;
using ShortUrl.Core.Models;

namespace ShortUrl.Core.Contracts
{
    public interface IReadShortUrlService
    {
        /// <summary>
        /// Get shortUrl from Cache or Database
        /// </summary>
        /// <param name="shortCode"></param>
        /// <returns></returns>
        Task<Result<ShortUrlSearchItem>> GetLongUrl(string shortCode, CancellationToken ct);
    }
}
