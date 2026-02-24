using ConnectBuySellToday.Domain.Common;
using ConnectBuySellToday.Domain.Interfaces;
using ConnectBuySellToday.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ConnectBuySellToday.Infrastructure.Repositories;

public class AdImageRepository : GenericRepository<AdImage>, IAdImageRepository
{
    private readonly new ApplicationDbContext _context;

    public AdImageRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<AdImage>> GetImagesByAdIdAsync(Guid adId)
    {
        return await _context.AdImages
            .Where(x => x.ProductAdId == adId)
            .ToListAsync();
    }

    public async Task SetMainImageAsync(Guid adId, Guid imageId)
    {
        // First, unset all images for this ad as non-main
        var images = await _context.AdImages
            .Where(x => x.ProductAdId == adId)
            .ToListAsync();

        foreach (var image in images)
        {
            image.IsMain = false;
        }

        // Set the specified image as main
        var mainImage = await _context.AdImages.FindAsync(imageId);
        if (mainImage != null)
        {
            mainImage.IsMain = true;
        }

        await _context.SaveChangesAsync();
    }
}
