using ConnectBuySellToday.Application.DTOs;
using ConnectBuySellToday.Application.Interfaces;
using ConnectBuySellToday.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ConnectBuySellToday.Web.Controllers;

public class AdsController : Controller
{
    private readonly IAdService _adService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AdsController> _logger;

    public AdsController(IAdService adService, IUnitOfWork unitOfWork, ILogger<AdsController> logger)
    {
        _adService = adService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<IActionResult> Index(string? searchQuery, Guid? categoryId)
    {
        var ads = await _adService.SearchAdsAsync(searchQuery, categoryId);
        return View(ads);
    }

    // GET: /Ads/Create
    public async Task<IActionResult> Create()
    {
        var categories = await _unitOfWork.Categories.GetAllAsync();
        ViewBag.Categories = categories;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AdDto adDto, IFormFile? ImageFile)
    {
        if (!ModelState.IsValid)
        {
            var categories = await _unitOfWork.Categories.GetAllAsync();
            ViewBag.Categories = categories;
            return View(adDto);
        }

        // Mocking a seller ID for now (usually comes from User.Identity)
        var sellerId = "user-123";

        // Handle file upload
        IEnumerable<IFormFile>? images = null;
        if (ImageFile != null && ImageFile.Length > 0)
        {
            images = new List<IFormFile> { ImageFile };
        }

        var success = await _adService.CreateAdAsync(adDto, sellerId, images);
        if (success) return RedirectToAction(nameof(Index));

        var allCategories = await _unitOfWork.Categories.GetAllAsync();
        ViewBag.Categories = allCategories;
        return View(adDto);
    }

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
