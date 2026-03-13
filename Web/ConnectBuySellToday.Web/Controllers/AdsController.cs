using ConnectBuySellToday.Application.DTOs;
using ConnectBuySellToday.Application.Interfaces;
using ConnectBuySellToday.Domain.Enums;
using ConnectBuySellToday.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using System.Security.Claims;

namespace ConnectBuySellToday.Web.Controllers;

[Authorize]
public class AdsController : Controller
{
    private readonly IAdService _adService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICategoryService _categoryService;
    private readonly ILogger<AdsController> _logger;
    private readonly IOutputCacheStore _outputCacheStore;

    public AdsController(IAdService adService, IUnitOfWork unitOfWork, ICategoryService categoryService, ILogger<AdsController> logger, IOutputCacheStore outputCacheStore)
    {
        _adService = adService;
        _unitOfWork = unitOfWork;
        _categoryService = categoryService;
        _logger = logger;
        _outputCacheStore = outputCacheStore;
    }

    [AllowAnonymous]
    public async Task<IActionResult> Index(string? searchQuery, Guid? categoryId, double? userLat, double? userLong, double? radiusInKm)
    {
        var ads = await _adService.SearchAdsAsync(searchQuery, categoryId, userLat, userLong, radiusInKm);
        var categories = await _categoryService.GetAllCategoriesAsync();
        ViewBag.Categories = categories;
        
        // Pass location values back to view for form
        ViewBag.UserLat = userLat;
        ViewBag.UserLong = userLong;
        ViewBag.RadiusInKm = radiusInKm;
        
        return View(ads);
    }

    // GET: /Ads/Create
    public async Task<IActionResult> Create()
{
var categories = await _categoryService.GetAllCategoriesAsync();
ViewBag.Categories = categories;
return View();
}

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AdDto adDto, List<IFormFile>? ImageFiles)
    {
        if (!ModelState.IsValid)
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            ViewBag.Categories = categories;
            return View(adDto);
        }

        // Get the current user ID from Identity
        var sellerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(sellerId))
        {
            return RedirectToAction("Login", "Account");
        }

        // Handle multiple file uploads - filter out empty files
        IEnumerable<IFormFile>? images = null;
        if (ImageFiles != null && ImageFiles.Count > 0)
        {
            images = ImageFiles.Where(f => f != null && f.Length > 0).ToList();
        }

        var success = await _adService.CreateAdAsync(adDto, sellerId, images);
        if (success) 
        {
            // Invalidate homepage cache when new ad is created
            await _outputCacheStore.EvictByTagAsync("home", default);
            return RedirectToAction(nameof(Index));
        }

        var allCategories = await _categoryService.GetAllCategoriesAsync();
        ViewBag.Categories = allCategories;
        return View(adDto);
    }


    [AllowAnonymous]
    public async Task<IActionResult> Details(Guid id)
    {
        var ad = await _adService.GetAdByIdAsync(id);
        
        if (ad == null)
        {
            return NotFound();
        }
        
        return View(ad);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleFavorite(Guid adId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Json(new { success = false, message = "Please login to add to favorites" });
        }

        try
        {
            var isFavorite = await _adService.ToggleFavoriteAsync(userId, adId);
            return Json(new { success = true, isFavorite = isFavorite });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling favorite for ad {AdId}", adId);
            return Json(new { success = false, message = "An error occurred" });
        }
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> IsFavorite(Guid adId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Json(new { isFavorite = false });
        }

        try
        {
            var isFavorite = await _adService.IsFavoriteAsync(userId, adId);
            return Json(new { isFavorite = isFavorite });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking favorite status for ad {AdId}", adId);
            return Json(new { isFavorite = false });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> ReportAd([FromBody] CreateReportDto createReportDto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Json(new { success = false, message = "Please login to report an ad" });
        }

        try
        {
            var reportService = HttpContext.RequestServices.GetRequiredService<IReportService>();
            var result = await reportService.ReportAdAsync(userId, createReportDto);
            
            if (result)
            {
                return Json(new { success = true, message = "Report submitted successfully. Thank you for your feedback." });
            }
            else
            {
                return Json(new { success = false, message = "You have already reported this ad or cannot report your own ad." });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reporting ad {AdId}", createReportDto.AdId);
            return Json(new { success = false, message = "An error occurred while submitting your report." });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> SubmitReview([FromBody] CreateReviewDto createReviewDto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Json(new { success = false, message = "Please login to submit a review" });
        }

        try
        {
            var reviewService = HttpContext.RequestServices.GetRequiredService<IReviewService>();
            
            // Check if user can review this seller
            var canReview = await reviewService.CanUserReviewSellerAsync(userId, createReviewDto.SellerId);
            if (!canReview)
            {
                return Json(new { success = false, message = "You must message the seller before leaving a review" });
            }

            var result = await reviewService.SubmitReviewAsync(userId, createReviewDto);
            
            if (result)
            {
                return Json(new { success = true, message = "Review submitted successfully!" });
            }
            else
            {
                return Json(new { success = false, message = "Failed to submit review. Please try again." });
            }
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "User {UserId} attempted to review without messaging seller", userId);
            return Json(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting review for seller {SellerId}", createReviewDto.SellerId);
            return Json(new { success = false, message = "An error occurred while submitting your review." });
        }
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> CanReviewSeller(string sellerId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Json(new { canReview = false, message = "Please login to review" });
        }

        try
        {
            var reviewService = HttpContext.RequestServices.GetRequiredService<IReviewService>();
            var canReview = await reviewService.CanUserReviewSellerAsync(userId, sellerId);
            return Json(new { canReview = canReview });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking review permission for seller {SellerId}", sellerId);
            return Json(new { canReview = false });
        }
    }
}
