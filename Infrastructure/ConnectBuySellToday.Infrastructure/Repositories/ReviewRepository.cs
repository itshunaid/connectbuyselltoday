using ConnectBuySellToday.Domain.Entities;
using ConnectBuySellToday.Domain.Interfaces;
using ConnectBuySellToday.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ConnectBuySellToday.Infrastructure.Repositories;

public class ReviewRepository : GenericRepository<Review>, IReviewRepository
{
    private readonly new ApplicationDbContext _context;

    public ReviewRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Review>> GetReviewsForSellerAsync(string sellerId)
    {
        return await _context.Reviews
            .Where(r => r.SellerId == sellerId)
            .Include(r => r.Buyer)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<Review?> GetReviewByBuyerAndSellerAsync(string buyerId, string sellerId)
    {
        return await _context.Reviews
            .FirstOrDefaultAsync(r => r.BuyerId == buyerId && r.SellerId == sellerId);
    }

    public async Task<double> GetAverageRatingForSellerAsync(string sellerId)
    {
        // Using SQL GroupBy to calculate average rating
        var result = await _context.Reviews
            .Where(r => r.SellerId == sellerId)
            .GroupBy(r => r.SellerId)
            .Select(g => new { AverageRating = g.Average(r => (double?)r.Rating) })
            .FirstOrDefaultAsync();

        return result?.AverageRating ?? 0;
    }

    public async Task<bool> HasMessagedSellerAsync(string buyerId, string sellerId)
    {
        // Check if the buyer has sent or received any messages from/to the seller
        return await _context.Messages
            .AnyAsync(m => 
                (m.SenderId == buyerId && m.ReceiverId == sellerId) ||
                (m.SenderId == sellerId && m.ReceiverId == buyerId));
    }

    public async Task<bool> UpdateAsync(Review review)
    {
        try
        {
            _context.Reviews.Update(review);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteAsync(Guid reviewId)
    {
        try
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review == null)
                return false;

            _context.Reviews.Remove(review);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
