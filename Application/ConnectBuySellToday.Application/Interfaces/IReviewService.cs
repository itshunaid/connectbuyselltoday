using ConnectBuySellToday.Application.DTOs;

namespace ConnectBuySellToday.Application.Interfaces;

public interface IReviewService
{
    Task<bool> SubmitReviewAsync(string buyerId, CreateReviewDto createReviewDto);
    Task<IEnumerable<ReviewDto>> GetReviewsForSellerAsync(string sellerId);
    Task<double> GetAverageRatingForSellerAsync(string sellerId);
    Task<bool> CanUserReviewSellerAsync(string buyerId, string sellerId);
    Task<ReviewDto?> GetUserReviewForSellerAsync(string buyerId, string sellerId);
}
