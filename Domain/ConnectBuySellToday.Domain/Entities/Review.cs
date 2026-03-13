using ConnectBuySellToday.Domain.Common;

namespace ConnectBuySellToday.Domain.Entities;

public class Review : BaseEntity
{
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public string BuyerId { get; set; } = string.Empty;
    public string SellerId { get; set; } = string.Empty;
    
    // Navigation properties
    public ApplicationUser Buyer { get; set; } = null!;
    public ApplicationUser Seller { get; set; } = null!;
}
