using ConnectBuySellToday.Application.DTOs;
using Microsoft.AspNetCore.Http;

namespace ConnectBuySellToday.Application.Interfaces;

public interface IAdService
{
    Task<IEnumerable<AdDto>> GetLatestAdsAsync(string? searchQuery = null);
    Task<IEnumerable<AdDto>> SearchAdsAsync(string? searchQuery, Guid? categoryId);
    Task<IEnumerable<AdDto>> GetAdsByCategoryAsync(Guid categoryId);
    Task<AdDto?> GetAdByIdAsync(Guid id);
    Task<bool> CreateAdAsync(AdDto adDto, string sellerId, IEnumerable<IFormFile>? images = null);
    Task<bool> UpdateAdAsync(AdDto adDto, string sellerId, IEnumerable<IFormFile>? newImages = null);
    Task<bool> DeleteAdAsync(Guid adId, string sellerId);
    Task<bool> AddImagesToAdAsync(Guid adId, IEnumerable<IFormFile> images, string sellerId);
    Task<bool> DeleteImageFromAdAsync(Guid adId, Guid imageId, string sellerId);
    Task<bool> SetMainImageAsync(Guid adId, Guid imageId, string sellerId);
}
