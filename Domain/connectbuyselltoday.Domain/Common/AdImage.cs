using ConnectBuySellToday.Domain.Entities;

namespace ConnectBuySellToday.Domain.Common
{
    public class AdImage : BaseEntity
    {
        public string Url { get; set; } = string.Empty;
        public bool IsMain { get; set; }

        public Guid ProductAdId { get; set; }
        public ProductAd ProductAd { get; set; } = null!;
    }
}
