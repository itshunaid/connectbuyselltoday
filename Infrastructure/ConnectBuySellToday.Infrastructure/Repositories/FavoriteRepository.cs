using ConnectBuySellToday.Domain.Entities;
using ConnectBuySellToday.Domain.Interfaces;
using ConnectBuySellToday.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ConnectBuySellToday.Infrastructure.Repositories;

public class FavoriteRepository : GenericRepository<Favorite>, IFavoriteRepository
{
    private readonly new ApplicationDbContext _context;

    public FavoriteRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<bool> IsFavoriteAsync(string userId, Guid productAdId)
    {
        return await _context.Favorites
            .AnyAsync(f => f.UserId == userId && f.ProductAdId == productAdId);
    }

    public async Task<IEnumerable<Favorite>> GetFavoritesByUserIdAsync(string userId)
    {
        return await _context.Favorites
            .Where(f => f.UserId == userId)
            .Include(f => f.ProductAd)
                .ThenInclude(p => p!.Images)
            .Include(f => f.ProductAd)
                .ThenInclude(p => p!.Category)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync();
    }

    public async Task RemoveFavoriteAsync(string userId, Guid productAdId)
    {
        var favorite = await _context.Favorites
            .FirstOrDefaultAsync(f => f.UserId == userId && f.ProductAdId == productAdId);

        if (favorite != null)
        {
            _context.Favorites.Remove(favorite);
        }
    }

    public async Task AddFavoriteAsync(string userId, Guid productAdId)
    {
        var favorite = new Favorite
        {
            UserId = userId,
            ProductAdId = productAdId,
            CreatedAt = DateTime.UtcNow
        };
        
        await _context.Favorites.AddAsync(favorite);
    }
}
