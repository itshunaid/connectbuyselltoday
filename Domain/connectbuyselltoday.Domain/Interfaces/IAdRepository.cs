using ConnectBuySellToday.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectBuySellToday.Domain.Interfaces
{
public interface IAdRepository : IGenericRepository<ProductAd>
{
    Task<IEnumerable<ProductAd>> GetAdsByCategoryAsync(Guid categoryId);
    Task<IEnumerable<ProductAd>> GetRecentAdsAsync(int count);
    Task<IEnumerable<ProductAd>> SearchAdsAsync(string searchTerm);
    Task<IEnumerable<ProductAd>> GetAdsBySellerIdAsync(string sellerId);
    Task<ProductAd?> GetAdByIdWithDetailsAsync(Guid id);
    
    // New method for efficient DB-level filtering
    Task<IEnumerable<ProductAd>> GetFilteredAdsAsync(string? search, Guid? categoryId);
    Task<IEnumerable<ProductAd>> GetAdsByUserIdAsync(string userId);
    
    // Admin methods
    Task<IEnumerable<ProductAd>> GetAllAdsAsync();
    Task<IEnumerable<ProductAd>> GetPendingAdsAsync();
    Task<int> GetTotalAdsCountAsync();
    Task<int> GetActiveAdsCountAsync();
    Task<int> GetPendingAdsCountAsync();
    Task<int> GetSoldAdsCountAsync();

    // Favorite methods
    Task<IEnumerable<ProductAd>> GetFavoriteAdsByUserIdAsync(string userId);
}
}
