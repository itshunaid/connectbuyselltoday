using ConnectBuySellToday.Application.Interfaces;
using ConnectBuySellToday.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using System.Diagnostics;

namespace ConnectBuySellToday.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAdService _adService;

        public HomeController(ILogger<HomeController> logger, IAdService adService)
        {
            _logger = logger;
            _adService = adService;
        }

        [OutputCache(Duration = 60, Tags = new[] { "home" })]
        public async Task<IActionResult> Index()
        {
            var ads = await _adService.GetLatestAdsAsync();
            return View(ads);
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
