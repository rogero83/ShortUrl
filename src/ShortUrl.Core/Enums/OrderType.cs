namespace ShortUrl.Core.Enums;

/// <summary>
/// Specifies the available ordering options for sorting collections of items, such as by creation date, expiration
/// date, or click counts, in ascending or descending order.
/// </summary>
/// <remarks>Use this enumeration to indicate how results should be ordered when retrieving or displaying
/// collections. Each value represents a specific field and sort direction. For example, use CreatedAtDesc to sort items
/// by creation date in descending order (most recent first).</remarks>
public enum OrderType
{
    /// <summary>
    /// Gets a value that indicates whether items are sorted by creation date in descending order.  
    /// </summary>
    CreatedAtDesc,
    /// <summary>
    /// Specifies that items are ordered by creation date in ascending order.
    /// </summary>
    CreatedAtAsc,
    /// <summary>
    /// Gets or sets the expiration date and time in descending order.
    /// </summary>
    ExpireAtDesc,
    /// <summary>
    /// Sorts items in ascending order based on their expiration time.
    /// </summary>
    ExpireAtAsc,
    /// <summary>
    /// Sorts items in descending order based on the total number of clicks.
    /// </summary>
    TotalClicksDesc,
    /// <summary>
    /// Sorts items by the total number of clicks in ascending order.
    /// </summary>
    TotalClicksAsc,
    /// <summary>
    /// Specifies that items are sorted by the number of unique clicks in descending order.
    /// </summary>
    UniqueClicksDesc,
    /// <summary>
    /// Sorts results by the number of unique clicks in ascending order.
    /// </summary>
    UniqueClicksAsc
}