using ConnectBuySellToday.Domain.Entities;

namespace ConnectBuySellToday.Domain.Interfaces;

/// <summary>
/// Repository interface for Favorite operations
/// </summary>
public interface IFavoriteRepository : IGenericRepository<Favorite>
{
    /// <summary>
    /// Check if a favorite exists for the given user and product ad
    /// </summary>
    Task<bool> IsFavoriteAsync(string userId, Guid productAdId);

    /// <summary>
    /// Get all favorites for a specific user
    /// </summary>
    Task<IEnumerable<Favorite>> GetFavoritesByUserIdAsync(string userId);

    /// <summary>
    /// Remove a favorite by userId and productAdId
    /// </summary>
    Task RemoveFavoriteAsync(string userId, Guid productAdId);

    /// <summary>
    /// Add a favorite for user and product ad
    /// </summary>
    Task AddFavoriteAsync(string userId, Guid productAdId);
}
