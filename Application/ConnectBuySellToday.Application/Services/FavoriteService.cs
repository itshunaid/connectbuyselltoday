using ConnectBuySellToday.Application.DTOs;
using ConnectBuySellToday.Application.Interfaces;
using ConnectBuySellToday.Domain.Interfaces;

namespace ConnectBuySellToday.Application.Services;

/// <summary>
/// Service implementation for managing user favorites/watchlist
/// </summary>
public class FavoriteService : IFavoriteService
{
    private readonly IUnitOfWork _unitOfWork;

    public FavoriteService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Toggles a product ad in/out of user's favorites/watchlist
    /// </summary>
    public async Task<bool> ToggleFavoriteAsync(string userId, Guid productAdId)
    {
        // Check if already favorited
        var existingFavorite = await _unitOfWork.Favorites.IsFavoriteAsync(userId, productAdId);
        
        if (existingFavorite)
        {
            // Remove from favorites
            await _unitOfWork.Favorites.RemoveFavoriteAsync(userId, productAdId);
            await _unitOfWork.CompleteAsync();
            return false;
        }
        else
        {
            // Add to favorites
            await _unitOfWork.Favorites.AddFavoriteAsync(userId, productAdId);
            await _unitOfWork.CompleteAsync();
            return true;
        }
    }

    /// <summary>
    /// Check if a product ad is in user's favorites
    /// </summary>
    public async Task<bool> IsFavoriteAsync(string userId, Guid productAdId)
    {
        return await _unitOfWork.Favorites.IsFavoriteAsync(userId, productAdId);
    }

    /// <summary>
    /// Get all favorite ads for a user
    /// </summary>
    public async Task<IEnumerable<AdDto>> GetUserFavoritesAsync(string userId)
    {
        var favoriteAds = await _unitOfWork.Ads.GetFavoriteAdsByUserIdAsync(userId);
        
        return favoriteAds.Select(ad => new AdDto
        {
            Id = ad.Id,
            Title = ad.Title,
            Description = ad.Description,
            Price = ad.Price,
            Status = ad.Status,
            CategoryId = ad.CategoryId,
            CategoryName = ad.Category?.Name ?? string.Empty,
            SellerId = ad.SellerId ?? string.Empty,
            MainImageUrl = ad.Images.FirstOrDefault()?.Url,
            ImageUrls = ad.Images.Select(i => i.Url ?? string.Empty).ToList(),
            CreatedAt = ad.CreatedAt
        });
    }
}
