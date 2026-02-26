﻿using ConnectBuySellToday.Application.DTOs;
using Microsoft.AspNetCore.Http;

namespace ConnectBuySellToday.Application.Interfaces;

public interface IAdService
{
    Task<IEnumerable<AdDto>> GetLatestAdsAsync(string? searchQuery = null);
    Task<IEnumerable<AdDto>> SearchAdsAsync(string? searchQuery, Guid? categoryId, double? userLatitude = null, double? userLongitude = null, double? radiusInKm = null);
    Task<IEnumerable<AdDto>> GetAdsByCategoryAsync(Guid categoryId);
    Task<AdDto?> GetAdByIdAsync(Guid id);
    Task<bool> CreateAdAsync(AdDto adDto, string sellerId, IEnumerable<IFormFile>? images = null);
    Task<bool> UpdateAdAsync(AdDto adDto, string sellerId, IEnumerable<IFormFile>? newImages = null);
    Task<bool> DeleteAdAsync(Guid adId, string sellerId);
    Task<bool> AddImagesToAdAsync(Guid adId, IEnumerable<IFormFile> images, string sellerId);
    Task<bool> DeleteImageFromAdAsync(Guid adId, Guid imageId, string sellerId);
    Task<bool> SetMainImageAsync(Guid adId, Guid imageId, string sellerId);
    Task<IEnumerable<AdDto>> GetAdsByUserIdAsync(string userId);
    Task<IEnumerable<AdDto>> GetAdsBySellerIdAsync(string sellerId);
    
    // Featured ads management
    Task<bool> FeatureAdAsync(Guid adId, bool isFeatured, DateTime? expiryDate);
}
