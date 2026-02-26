﻿using ConnectBuySellToday.Application.DTOs;
using ConnectBuySellToday.Application.Interfaces;
using ConnectBuySellToday.Domain.Common;
using ConnectBuySellToday.Domain.Entities;
using ConnectBuySellToday.Domain.Interfaces;
using Microsoft.AspNetCore.Http;

namespace ConnectBuySellToday.Application.Services;

public class AdService : IAdService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IImageService _imageService;

    public AdService(IUnitOfWork unitOfWork, IImageService imageService)
    {
        _unitOfWork = unitOfWork;
        _imageService = imageService;
    }

    public async Task<IEnumerable<AdDto>> GetLatestAdsAsync(string? searchQuery = null)
    {
        var ads = await _unitOfWork.Ads.GetRecentAdsAsync(10);

        return ads.Select(a => new AdDto
        {
            Id = a.Id,
            Title = a.Title,
            Description = a.Description,
            Price = a.Price,
            Status = a.Status,
            CategoryId = a.CategoryId,
            CategoryName = a.Category?.Name ?? "General",
            SellerId = a.SellerId,
            MainImageUrl = a.Images.FirstOrDefault(i => i.IsMain)?.Url ?? a.Images.FirstOrDefault()?.Url,
            ImageUrls = a.Images.Select(i => i.Url).ToList(),
            CreatedAt = a.CreatedAt,
            Latitude = a.Location?.Latitude,
            Longitude = a.Location?.Longitude,
            CityName = a.Location?.CityName,
            IsFeatured = a.IsFeatured,
            FeaturedExpiryDate = a.FeaturedExpiryDate
        });
    }

    public async Task<IEnumerable<AdDto>> SearchAdsAsync(string? searchQuery, Guid? categoryId, double? userLatitude = null, double? userLongitude = null, double? radiusInKm = null)
    {
        // Use the new efficient DB-level filtering method with location parameters
        var ads = await _unitOfWork.Ads.GetFilteredAdsAsync(searchQuery, categoryId, userLatitude, userLongitude, radiusInKm);

        return ads.Select(a => new AdDto
        {
            Id = a.Id,
            Title = a.Title,
            Description = a.Description,
            Price = a.Price,
            Status = a.Status,
            CategoryId = a.CategoryId,
            CategoryName = a.Category?.Name ?? "General",
            SellerId = a.SellerId,
            MainImageUrl = a.Images.FirstOrDefault(i => i.IsMain)?.Url ?? a.Images.FirstOrDefault()?.Url,
            ImageUrls = a.Images.Select(i => i.Url).ToList(),
            CreatedAt = a.CreatedAt,
            Latitude = a.Location?.Latitude,
            Longitude = a.Location?.Longitude,
            CityName = a.Location?.CityName,
            IsFeatured = a.IsFeatured,
            FeaturedExpiryDate = a.FeaturedExpiryDate
        });
    }

    public async Task<IEnumerable<AdDto>> GetAdsByCategoryAsync(Guid categoryId)
    {
        var ads = await _unitOfWork.Ads.GetAdsByCategoryAsync(categoryId);

        return ads.Select(a => new AdDto
        {
            Id = a.Id,
            Title = a.Title,
            Description = a.Description,
            Price = a.Price,
            Status = a.Status,
            CategoryId = a.CategoryId,
            CategoryName = a.Category?.Name ?? "General",
            SellerId = a.SellerId,
            MainImageUrl = a.Images.FirstOrDefault(i => i.IsMain)?.Url ?? a.Images.FirstOrDefault()?.Url,
            ImageUrls = a.Images.Select(i => i.Url).ToList(),
            CreatedAt = a.CreatedAt,
            Latitude = a.Location?.Latitude,
            Longitude = a.Location?.Longitude,
            CityName = a.Location?.CityName,
            IsFeatured = a.IsFeatured,
            FeaturedExpiryDate = a.FeaturedExpiryDate
        });
    }

    public async Task<AdDto?> GetAdByIdAsync(Guid id)
    {
        var ad = await _unitOfWork.Ads.GetAdByIdWithDetailsAsync(id);
        
        if (ad == null)
            return null;

        return new AdDto
        {
            Id = ad.Id,
            Title = ad.Title,
            Description = ad.Description,
            Price = ad.Price,
            Status = ad.Status,
            CategoryId = ad.CategoryId,
            CategoryName = ad.Category?.Name ?? "General",
            SellerId = ad.SellerId,
            MainImageUrl = ad.Images.FirstOrDefault(i => i.IsMain)?.Url ?? ad.Images.FirstOrDefault()?.Url,
            ImageUrls = ad.Images.Select(i => i.Url).ToList(),
            CreatedAt = ad.CreatedAt,
            Latitude = ad.Location?.Latitude,
            Longitude = ad.Location?.Longitude,
            CityName = ad.Location?.CityName,
            IsFeatured = ad.IsFeatured,
            FeaturedExpiryDate = ad.FeaturedExpiryDate
        };
    }

    public async Task<bool> CreateAdAsync(AdDto adDto, string sellerId, IEnumerable<IFormFile>? images = null)
    {
        string imageUrl = "/images/no-image.png";

        // Handle image upload if provided
        if (images != null && images.Any())
        {
            var imageList = images.ToList();
            if (imageList.Any())
            {
                var firstImage = imageList.First();
                imageUrl = await _imageService.UploadImageAsync(firstImage, "ads");
            }
        }
        else if (adDto.ImageFile != null)
        {
            // Also check ImageFile property for backward compatibility
            imageUrl = await _imageService.UploadImageAsync(adDto.ImageFile, "ads");
        }

        var newAd = new ProductAd
        {
            Title = adDto.Title,
            Description = adDto.Description,
            Price = adDto.Price,
            SellerId = sellerId,
            CategoryId = adDto.CategoryId,
            Status = Domain.Enums.AdStatus.PendingReview,
            Location = (adDto.Latitude.HasValue && adDto.Longitude.HasValue) 
                ? new Location(adDto.Latitude.Value, adDto.Longitude.Value, adDto.CityName ?? string.Empty)
                : null
        };

        // Add the image
        newAd.Images.Add(new AdImage 
        { 
            Url = imageUrl, 
            IsMain = true 
        });

        await _unitOfWork.Ads.AddAsync(newAd);
        var result = await _unitOfWork.CompleteAsync();

        return result > 0;
    }

    public async Task<bool> UpdateAdAsync(AdDto adDto, string sellerId, IEnumerable<IFormFile>? newImages = null)
    {
        var ad = await _unitOfWork.Ads.GetByIdAsync(adDto.Id);
        
        if (ad == null || ad.SellerId != sellerId)
            return false;

        ad.Title = adDto.Title;
        ad.Description = adDto.Description;
        ad.Price = adDto.Price;
        ad.CategoryId = adDto.CategoryId;
        
        // Update location if provided
        if (adDto.Latitude.HasValue && adDto.Longitude.HasValue)
        {
            ad.Location = new Location(adDto.Latitude.Value, adDto.Longitude.Value, adDto.CityName ?? string.Empty);
        }

        // Handle new images if provided
        if (newImages != null && newImages.Any())
        {
            foreach (var imageFile in newImages)
            {
                var imageUrl = await _imageService.UploadImageAsync(imageFile, "ads");
                ad.Images.Add(new AdImage 
                { 
                    Url = imageUrl, 
                    IsMain = !ad.Images.Any() 
                });
            }
        }

        _unitOfWork.Ads.Update(ad);
        var result = await _unitOfWork.CompleteAsync();

        return result > 0;
    }

    public async Task<bool> DeleteAdAsync(Guid adId, string sellerId)
    {
        var ad = await _unitOfWork.Ads.GetByIdAsync(adId);
        
        if (ad == null || ad.SellerId != sellerId)
            return false;

        // Delete all images associated with the ad
        foreach (var image in ad.Images)
        {
            await _imageService.DeleteImageAsync(image.Url);
        }

        _unitOfWork.Ads.Delete(ad);
        var result = await _unitOfWork.CompleteAsync();

        return result > 0;
    }

    public async Task<bool> AddImagesToAdAsync(Guid adId, IEnumerable<IFormFile> images, string sellerId)
    {
        var ad = await _unitOfWork.Ads.GetAdByIdWithDetailsAsync(adId);
        
        if (ad == null || ad.SellerId != sellerId)
            return false;

        foreach (var imageFile in images)
        {
            var imageUrl = await _imageService.UploadImageAsync(imageFile, "ads");
            ad.Images.Add(new AdImage 
            { 
                Url = imageUrl, 
                IsMain = !ad.Images.Any() 
            });
        }

        _unitOfWork.Ads.Update(ad);
        var result = await _unitOfWork.CompleteAsync();

        return result > 0;
    }

    public async Task<bool> DeleteImageFromAdAsync(Guid adId, Guid imageId, string sellerId)
    {
        var ad = await _unitOfWork.Ads.GetAdByIdWithDetailsAsync(adId);
        
        if (ad == null || ad.SellerId != sellerId)
            return false;

        var image = ad.Images.FirstOrDefault(i => i.Id == imageId);
        
        if (image == null)
            return false;

        // Delete the physical file
        await _imageService.DeleteImageAsync(image.Url);

        // Remove from database
        ad.Images.Remove(image);
        
        // If deleted image was main and there are other images, set a new main
        if (image.IsMain && ad.Images.Any())
        {
            ad.Images.First().IsMain = true;
        }

        _unitOfWork.Ads.Update(ad);
        var result = await _unitOfWork.CompleteAsync();

        return result > 0;
    }

    public async Task<bool> SetMainImageAsync(Guid adId, Guid imageId, string sellerId)
    {
        var ad = await _unitOfWork.Ads.GetAdByIdWithDetailsAsync(adId);
        
        if (ad == null || ad.SellerId != sellerId)
            return false;

        var image = ad.Images.FirstOrDefault(i => i.Id == imageId);
        
        if (image == null)
            return false;

        // Unset all main images
        foreach (var img in ad.Images)
        {
            img.IsMain = false;
        }

        // Set the selected image as main
        image.IsMain = true;

        _unitOfWork.Ads.Update(ad);
        var result = await _unitOfWork.CompleteAsync();

        return result > 0;
    }

    public async Task<IEnumerable<AdDto>> GetAdsByUserIdAsync(string userId)
    {
        var ads = await _unitOfWork.Ads.GetAdsByUserIdAsync(userId);

        return ads.Select(a => new AdDto
        {
            Id = a.Id,
            Title = a.Title,
            Description = a.Description,
            Price = a.Price,
            Status = a.Status,
            CategoryId = a.CategoryId,
            CategoryName = a.Category?.Name ?? "General",
            SellerId = a.SellerId,
            MainImageUrl = a.Images.FirstOrDefault(i => i.IsMain)?.Url ?? a.Images.FirstOrDefault()?.Url,
            ImageUrls = a.Images.Select(i => i.Url).ToList(),
            CreatedAt = a.CreatedAt,
            Latitude = a.Location?.Latitude,
            Longitude = a.Location?.Longitude,
            CityName = a.Location?.CityName,
            IsFeatured = a.IsFeatured,
            FeaturedExpiryDate = a.FeaturedExpiryDate
        });
    }

    public async Task<IEnumerable<AdDto>> GetAdsBySellerIdAsync(string sellerId)
    {
        // Reuse the existing method
        return await GetAdsByUserIdAsync(sellerId);
    }

    public async Task<bool> FeatureAdAsync(Guid adId, bool isFeatured, DateTime? expiryDate)
    {
        var ad = await _unitOfWork.Ads.GetByIdAsync(adId);
        
        if (ad == null)
            return false;

        // Validation: If featuring an ad, expiry date must be provided and in the future
        if (isFeatured)
        {
            if (!expiryDate.HasValue)
                throw new ArgumentException("Expiry date is required when featuring an ad");
            
            if (expiryDate.Value <= DateTime.UtcNow)
                throw new ArgumentException("Expiry date must be in the future");
        }

        ad.IsFeatured = isFeatured;
        ad.FeaturedExpiryDate = isFeatured ? expiryDate : null;

        _unitOfWork.Ads.Update(ad);
        var result = await _unitOfWork.CompleteAsync();

        return result > 0;
    }
}
