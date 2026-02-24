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
    }
}
