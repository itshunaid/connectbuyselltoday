using ConnectBuySellToday.Domain.Entities;

namespace ConnectBuySellToday.Domain.Interfaces;

public interface IReviewRepository
{
    Task<IEnumerable<Review>> GetReviewsForSellerAsync(string sellerId);
    Task<Review?> GetReviewByBuyerAndSellerAsync(string buyerId, string sellerId);
    Task<double> GetAverageRatingForSellerAsync(string sellerId);
    Task<bool> HasMessagedSellerAsync(string buyerId, string sellerId);
    Task AddAsync(Review review);
    Task<bool> UpdateAsync(Review review);
    Task<bool> DeleteAsync(Guid reviewId);
}
