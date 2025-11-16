using System.ComponentModel.DataAnnotations;

namespace ShortUrl.Core.Models;

public record ShortCodesRequest(int Page = 1,
    [Range(10, 100)] int ItemByPage = 20,
    OrderType OrderField = OrderType.CreatedAtDesc);

public enum OrderType
{
    CreatedAtDesc,
    CreatedAtAsc,
    ExpireAtDesc,
    ExpireAtAsc,
    TotalClicksDesc,
    TotalClicksAsc,
    UniqueClicksDesc,
    UniqueClicksAsc
}