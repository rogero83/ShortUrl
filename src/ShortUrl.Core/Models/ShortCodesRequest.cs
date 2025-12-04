using ShortUrl.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace ShortUrl.Core.Models;

/// <summary>
/// Represents the parameters used to request a paginated, ordered list of short codes.
/// </summary>
/// <param name="Page">The page number to retrieve. Must be greater than or equal to 1.</param>
/// <param name="ItemByPage">The number of items to include per page. Must be between 10 and 100, inclusive.</param>
/// <param name="OrderField">The field and direction by which to order the results.</param>
public record ShortCodesRequest(int Page = 1,
    [Range(10, 100)] int ItemByPage = 20,
    OrderType OrderField = OrderType.CreatedAtDesc);
