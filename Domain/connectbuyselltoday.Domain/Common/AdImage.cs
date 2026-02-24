using connectbuyselltoday.Domain.Entities;

namespace connectbuyselltoday.Domain.Common
{
    public class AdImage : BaseEntity
    {
        public string Url { get; set; } = string.Empty;
        public bool IsMain { get; set; }

        public Guid ProductAdId { get; set; }
        public ProductAd ProductAd { get; set; } = null!;
    }
}
