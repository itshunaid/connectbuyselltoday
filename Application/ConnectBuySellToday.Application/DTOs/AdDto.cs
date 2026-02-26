﻿﻿﻿using ConnectBuySellToday.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace ConnectBuySellToday.Application.DTOs;

public class AdDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public AdStatus Status { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string SellerId { get; set; } = string.Empty;
    public string? MainImageUrl { get; set; }
    public List<string> ImageUrls { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    
    // Location properties
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? CityName { get; set; }
    
    // Featured ad properties
    public bool IsFeatured { get; set; }
    public DateTime? FeaturedExpiryDate { get; set; }
    
    // For file upload
    public IFormFile? ImageFile { get; set; }
    
    // For multiple image uploads
    public List<IFormFile>? ImageFiles { get; set; }
}
