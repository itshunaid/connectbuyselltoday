using ConnectBuySellToday.Domain.Common;

namespace ConnectBuySellToday.Domain.Entities;

/// <summary>
/// Represents a favorite/wishlist item linking a user to a product ad
/// </summary>
public class Favorite : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    public Guid ProductAdId { get; set; }
    public ProductAd ProductAd { get; set; } = null!;
}
