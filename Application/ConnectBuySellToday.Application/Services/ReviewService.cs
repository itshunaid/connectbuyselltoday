using ConnectBuySellToday.Application.DTOs;
using ConnectBuySellToday.Application.Interfaces;
using ConnectBuySellToday.Domain.Entities;
using ConnectBuySellToday.Domain.Interfaces;

namespace ConnectBuySellToday.Application.Services;

public class ReviewService : IReviewService
{
    private readonly IUnitOfWork _unitOfWork;

    public ReviewService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> SubmitReviewAsync(string buyerId, CreateReviewDto createReviewDto)
    {
        // Validate rating
        if (createReviewDto.Rating < 1 || createReviewDto.Rating > 5)
        {
            throw new ArgumentException("Rating must be between 1 and 5");
        }

        // Check if buyer has messaged the seller
        var hasMessaged = await _unitOfWork.Reviews.HasMessagedSellerAsync(buyerId, createReviewDto.SellerId);
        if (!hasMessaged)
        {
            throw new InvalidOperationException("You must message the seller before leaving a review");
        }

        // Check if buyer has already reviewed this seller
        var existingReview = await _unitOfWork.Reviews.GetReviewByBuyerAndSellerAsync(buyerId, createReviewDto.SellerId);
        if (existingReview != null)
        {
            // Update existing review
            existingReview.Rating = createReviewDto.Rating;
            existingReview.Comment = createReviewDto.Comment;
            return await _unitOfWork.Reviews.UpdateAsync(existingReview);
        }

        // Create new review
        var review = new Review
        {
            Rating = createReviewDto.Rating,
            Comment = createReviewDto.Comment,
            BuyerId = buyerId,
            SellerId = createReviewDto.SellerId,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Reviews.AddAsync(review);
        var result = await _unitOfWork.CompleteAsync();

        return result > 0;
    }

    public async Task<IEnumerable<ReviewDto>> GetReviewsForSellerAsync(string sellerId)
    {
        var reviews = await _unitOfWork.Reviews.GetReviewsForSellerAsync(sellerId);

        return reviews.Select(r => new ReviewDto
        {
            Id = r.Id,
            Rating = r.Rating,
            Comment = r.Comment,
            BuyerId = r.BuyerId,
            BuyerName = $"{r.Buyer.FirstName} {r.Buyer.LastName}".Trim(),
            SellerId = r.SellerId,
            CreatedAt = r.CreatedAt
        });
    }

    public async Task<double> GetAverageRatingForSellerAsync(string sellerId)
    {
        return await _unitOfWork.Reviews.GetAverageRatingForSellerAsync(sellerId);
    }

    public async Task<bool> CanUserReviewSellerAsync(string buyerId, string sellerId)
    {
        // Cannot review yourself
        if (buyerId == sellerId)
            return false;

        // Must have messaged the seller
        var hasMessaged = await _unitOfWork.Reviews.HasMessagedSellerAsync(buyerId, sellerId);
        return hasMessaged;
    }

    public async Task<ReviewDto?> GetUserReviewForSellerAsync(string buyerId, string sellerId)
    {
        var review = await _unitOfWork.Reviews.GetReviewByBuyerAndSellerAsync(buyerId, sellerId);
        
        if (review == null)
            return null;

        return new ReviewDto
        {
            Id = review.Id,
            Rating = review.Rating,
            Comment = review.Comment,
            BuyerId = review.BuyerId,
            SellerId = review.SellerId,
            CreatedAt = review.CreatedAt
        };
    }
}
