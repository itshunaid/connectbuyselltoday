using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using connectbuyselltoday.Domain.Common;
using connectbuyselltoday.Domain.Enums;
using System.Net.NetworkInformation;
using static System.Net.Mime.MediaTypeNames;

namespace connectbuyselltoday.Domain.Entities
{
    public class ProductAd : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public AdStatus Status { get; set; } = AdStatus.Active;

        // Foreign Keys
        public Guid CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        // In Clean Architecture, we often store the Identity User ID as a string 
        // to keep the Domain layer decoupled from the Identity Framework.
        public string SellerId { get; set; } = string.Empty;

        public List<AdImage> Images { get; set; } = new();
        public List<Message> Messages { get; set; } = new();
    }
}
