using ShortUrl.Common.ResultPattern;
using ShortUrl.Core.Models;

namespace ShortUrl.Core.Contracts
{
    public interface IShortUrlService
    {
        /// <summary>
        /// Creates a new short URL based on the specified request and API key context.
        /// </summary>
        /// <param name="apiKeyContext">The context containing API key information used to authorize the request. Cannot be null.</param>
        /// <param name="request">The details of the short URL to create, including the original URL and any optional parameters. Cannot be
        /// null.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Result{T}"/> with a
        /// <see cref="CreateShortUrlResponse"/> describing the created short URL if successful, or error information if
        /// the operation fails.</returns>
        Task<Result<CreateShortUrlResponse>> CreateShortUrl(ApiKeyContext apiKeyContext, CreateShortUrlRequest request, CancellationToken ct);

        /// <summary>
        /// Edits the properties of an existing short URL identified by its short code.
        /// </summary>
        /// <param name="apiKeyContext">The API key context used to authorize the edit operation. Must not be null.</param>
        /// <param name="shortCode">The unique short code that identifies the short URL to edit. Cannot be null or empty.</param>
        /// <param name="request">An object containing the new values for the short URL's properties. Must not be null.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a Result object with the
        /// response data for the edited short URL.</returns>
        Task<Result<EditShortUrlResponse>> EditShortUrl(ApiKeyContext apiKeyContext, string shortCode, EditShortUrlRequest request, CancellationToken ct);

        /// <summary>
        /// Retrieves a list of short URLs based on the specified request parameters.
        /// </summary>
        /// <param name="current">The API key context used to authorize the request. Cannot be null.</param>
        /// <param name="request">The parameters that define the filtering and pagination options for the short URLs to retrieve. Cannot be
        /// null.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a Result object with the list of
        /// short URLs and related metadata.</returns>
        Task<Result<ShortCodesResponse>> ListShortUrls(ApiKeyContext current, ShortCodesRequest request, CancellationToken ct);
    }
}
