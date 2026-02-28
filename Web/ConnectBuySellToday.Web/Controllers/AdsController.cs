using ConnectBuySellToday.Application.DTOs;
using ConnectBuySellToday.Application.Interfaces;
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
}
