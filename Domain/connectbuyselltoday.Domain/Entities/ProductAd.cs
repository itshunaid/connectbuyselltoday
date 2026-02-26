using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConnectBuySellToday.Domain.Common;
using ConnectBuySellToday.Domain.Enums;
using System.Net.NetworkInformation;
using static System.Net.Mime.MediaTypeNames;

namespace ConnectBuySellToday.Domain.Entities
{
    public class ProductAd : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public AdStatus Status { get; set; } = AdStatus.Active;

        // Location value object
        public Location? Location { get; set; }

        // Foreign Keys
        public Guid CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        // In Clean Architecture, we often store the Identity User ID as a string 
        // to keep the Domain layer decoupled from the Identity Framework.
        // Made nullable to support SetNull delete behavior when a user is deleted
        public string? SellerId { get; set; }
        
        // Navigation property for EF Core relationship
        public ApplicationUser? Seller { get; set; }

        public List<AdImage> Images { get; set; } = new();
        public List<Message> Messages { get; set; } = new();
        public List<Favorite> Favorites { get; set; } = new();
    }
}
