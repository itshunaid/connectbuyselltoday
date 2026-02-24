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
}
