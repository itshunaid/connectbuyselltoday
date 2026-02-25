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
    public async Task<IActionResult> Index(string? searchQuery, Guid? categoryId)
    {
        var ads = await _adService.SearchAdsAsync(searchQuery, categoryId);
        var categories = await _categoryService.GetAllCategoriesAsync();
        ViewBag.Categories = categories;
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
    public async Task<IActionResult> Create(AdDto adDto, IFormFile? ImageFile)
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

        // Handle file upload
        IEnumerable<IFormFile>? images = null;
        if (ImageFile != null && ImageFile.Length > 0)
        {
            images = new List<IFormFile> { ImageFile };
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
}
