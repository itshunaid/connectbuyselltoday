using ConnectBuySellToday.Application.Interfaces;
using ConnectBuySellToday.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;

namespace ConnectBuySellToday.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAdService _adService;
        private readonly ICategoryService _categoryService;
        private readonly IMemoryCache _memoryCache;

        private const string LatestAdsCacheKey = "LatestAdsCache";
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);

        public HomeController(
            ILogger<HomeController> logger, 
            IAdService adService, 
            ICategoryService categoryService,
            IMemoryCache memoryCache)
        {
            _logger = logger;
            _adService = adService;
            _categoryService = categoryService;
            _memoryCache = memoryCache;
        }

        [ResponseCache(Duration = 300, Location = ResponseCacheLocation.Client, VaryByHeader = "User-Agent")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _categoryService.GetRootCategoriesAsync();
            return PartialView("_CategoryList", categories);
        }

        [OutputCache(Duration = 60, Tags = new[] { "home" })]
        public async Task<IActionResult> Index()
        {
            // Try to get from memory cache first
            if (!_memoryCache.TryGetValue(LatestAdsCacheKey, out IEnumerable<ConnectBuySellToday.Application.DTOs.AdDto>? cachedAds))
            {
                cachedAds = await _adService.GetLatestAdsAsync();
                
                // Cache the result for 10 minutes
                _memoryCache.Set(LatestAdsCacheKey, cachedAds, CacheDuration);
            }
            
            var categories = await _categoryService.GetRootCategoriesAsync();
            ViewBag.Categories = categories;
            return View(cachedAds);
        }

        // Public method to invalidate the cache (to be called by AdminController)
        public static void InvalidateLatestAdsCache(IMemoryCache cache)
        {
            cache.Remove(LatestAdsCacheKey);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
