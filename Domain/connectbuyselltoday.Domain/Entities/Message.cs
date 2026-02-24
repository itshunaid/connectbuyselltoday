using connectbuyselltoday.Domain.Common;

namespace connectbuyselltoday.Domain.Entities
{
    public class Message : BaseEntity
    {
        public string Content { get; set; } = string.Empty;
        public string SenderId { get; set; } = string.Empty;
        public string ReceiverId { get; set; } = string.Empty;

        public Guid ProductAdId { get; set; }
        public ProductAd ProductAd { get; set; } = null!;
    }
}
