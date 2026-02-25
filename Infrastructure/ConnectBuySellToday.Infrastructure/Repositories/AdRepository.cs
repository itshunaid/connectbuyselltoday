using ConnectBuySellToday.Domain.Entities;
using ConnectBuySellToday.Domain.Interfaces;
using ConnectBuySellToday.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ConnectBuySellToday.Infrastructure.Repositories;

public class AdRepository : GenericRepository<ProductAd>, IAdRepository
{
    private readonly new ApplicationDbContext _context;

    public AdRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ProductAd>> GetAdsByCategoryAsync(Guid categoryId)
    {
        return await _context.ProductAds
            .Where(x => x.CategoryId == categoryId)
            .Include(x => x.Category)
            .Include(x => x.Images)
            .ToListAsync();
    }

    public async Task<IEnumerable<ProductAd>> SearchAdsAsync(string searchTerm)
    {
        return await _context.ProductAds
            .Where(x => x.Title.Contains(searchTerm) || x.Description.Contains(searchTerm))
            .Include(x => x.Category)
            .Include(x => x.Images)
            .ToListAsync();
    }

    public async Task<IEnumerable<ProductAd>> GetRecentAdsAsync(int count)
    {
        return await _context.ProductAds
            .Where(x => x.Status == Domain.Enums.AdStatus.Active)
            .OrderByDescending(x => x.CreatedAt)
            .Take(count)
            .Include(x => x.Category)
            .Include(x => x.Images)
            .ToListAsync();
    }

    public async Task<IEnumerable<ProductAd>> GetAdsBySellerIdAsync(string sellerId)
    {
        return await _context.ProductAds
            .Where(x => x.SellerId == sellerId)
            .Include(x => x.Images)
            .ToListAsync();
    }

    public async Task<ProductAd?> GetAdByIdWithDetailsAsync(Guid id)
    {
        return await _context.ProductAds
            .Include(x => x.Category)
            .Include(x => x.Images)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<ProductAd>> GetFilteredAdsAsync(string? search, Guid? categoryId)
    {
        IQueryable<ProductAd> query = _context.ProductAds
            .Include(x => x.Images)
            .Include(x => x.Category)
            .Where(x => x.Status == Domain.Enums.AdStatus.Active);

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(x => x.Title.Contains(search) || x.Description.Contains(search));
        }

        if (categoryId.HasValue)
        {
            query = query.Where(x => x.CategoryId == categoryId.Value);
        }

        return await query.OrderByDescending(x => x.CreatedAt).ToListAsync();
    }

    public async Task<IEnumerable<ProductAd>> GetAdsByUserIdAsync(string userId)
    {
        return await _context.ProductAds
            .Where(x => x.SellerId == userId)
            .Include(x => x.Category)
            .Include(x => x.Images)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<ProductAd>> GetAllAdsAsync()
    {
        return await _context.ProductAds
            .Include(x => x.Category)
            .Include(x => x.Images)
            .Include(x => x.Seller)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<ProductAd>> GetPendingAdsAsync()
    {
        return await _context.ProductAds
            .Where(x => x.Status == Domain.Enums.AdStatus.PendingReview)
            .Include(x => x.Category)
            .Include(x => x.Images)
            .Include(x => x.Seller)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<int> GetTotalAdsCountAsync()
    {
        return await _context.ProductAds.CountAsync();
    }

    public async Task<int> GetActiveAdsCountAsync()
    {
        return await _context.ProductAds
            .CountAsync(x => x.Status == Domain.Enums.AdStatus.Active);
    }

    public async Task<int> GetPendingAdsCountAsync()
    {
        return await _context.ProductAds
            .CountAsync(x => x.Status == Domain.Enums.AdStatus.PendingReview);
    }

    public async Task<int> GetSoldAdsCountAsync()
    {
        return await _context.ProductAds
            .CountAsync(x => x.Status == Domain.Enums.AdStatus.Sold);
    }

    public async Task<IEnumerable<ProductAd>> GetFavoriteAdsByUserIdAsync(string userId)
    {
        return await _context.ProductAds
            .Where(x => x.Favorites.Any(f => f.UserId == userId))
            .Include(x => x.Category)
            .Include(x => x.Images)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }
}
