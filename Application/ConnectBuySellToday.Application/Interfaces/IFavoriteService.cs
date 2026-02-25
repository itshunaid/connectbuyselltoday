using ConnectBuySellToday.Application.DTOs;

namespace ConnectBuySellToday.Application.Interfaces;

/// <summary>
/// Service interface for managing user favorites/watchlist
/// </summary>
public interface IFavoriteService
{
    /// <summary>
    /// Toggles a product ad in/out of user's favorites/watchlist
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="productAdId">The product ad ID</param>
    /// <returns>True if the ad is now a favorite, false if it was removed</returns>
    Task<bool> ToggleFavoriteAsync(string userId, Guid productAdId);

    /// <summary>
    /// Check if a product ad is in user's favorites
    /// </summary>
    Task<bool> IsFavoriteAsync(string userId, Guid productAdId);

    /// <summary>
    /// Get all favorite ads for a user
    /// </summary>
    Task<IEnumerable<AdDto>> GetUserFavoritesAsync(string userId);
}
