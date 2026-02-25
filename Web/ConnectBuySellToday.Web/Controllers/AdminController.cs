using ConnectBuySellToday.Application.DTOs;
using ConnectBuySellToday.Application.Interfaces;
using ConnectBuySellToday.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using System.Security.Claims;

namespace ConnectBuySellToday.Web.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly IAdminService _adminService;
    private readonly ILogger<AdminController> _logger;
    private readonly IOutputCacheStore _outputCacheStore;

    public AdminController(IAdminService adminService, ILogger<AdminController> logger, IOutputCacheStore outputCacheStore)
    {
        _adminService = adminService;
        _logger = logger;
        _outputCacheStore = outputCacheStore;
    }

    public async Task<IActionResult> Index()
    {
        var stats = await _adminService.GetDashboardStatsAsync();
        return View(stats);
    }

    // User Management
    public async Task<IActionResult> Users()
    {
        var users = await _adminService.GetAllUsersAsync();
        return View(users);
    }

    public async Task<IActionResult> UserDetails(string id)
    {
        var user = await _adminService.GetUserByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        return View(user);
    }

    [HttpPost]
    public async Task<IActionResult> BanUser(string userId, string reason, DateTime? banExpiresAt)
    {
        var result = await _adminService.BanUserAsync(userId, reason, banExpiresAt);
        if (result)
        {
            TempData["Success"] = "User has been banned successfully.";
        }
        else
        {
            TempData["Error"] = "Failed to ban user.";
        }
        return RedirectToAction("Users");
    }

    [HttpPost]
    public async Task<IActionResult> UnbanUser(string userId)
    {
        var result = await _adminService.UnbanUserAsync(userId);
        if (result)
        {
            TempData["Success"] = "User has been unbanned successfully.";
        }
        else
        {
            TempData["Error"] = "Failed to unban user.";
        }
        return RedirectToAction("Users");
    }

    // Ad Management
    public async Task<IActionResult> Ads(AdStatus? status)
    {
        var ads = await _adminService.GetAllAdsAsync(status);
        return View(ads);
    }

    public async Task<IActionResult> PendingAds()
    {
        var ads = await _adminService.GetPendingAdsAsync();
        return View(ads);
    }

    public async Task<IActionResult> ApproveAd(Guid id)
    {
        var result = await _adminService.ApproveAdAsync(id);
        if (result)
        {
            await _outputCacheStore.EvictByTagAsync("home", default);
            TempData["Success"] = "Ad has been approved.";
        }
        else
        {
            TempData["Error"] = "Failed to approve ad.";
        }
        return RedirectToAction("PendingAds");
    }

    [HttpPost]
    public async Task<IActionResult> RejectAd(Guid id, string reason)
    {
        var result = await _adminService.RejectAdAsync(id, reason);
        if (result)
        {
            await _outputCacheStore.EvictByTagAsync("home", default);
            TempData["Success"] = "Ad has been rejected.";
        }
        else
        {
            TempData["Error"] = "Failed to reject ad.";
        }
        return RedirectToAction("PendingAds");
    }

    public async Task<IActionResult> HideAd(Guid id)
    {
        var result = await _adminService.HideAdAsync(id);
        if (result)
        {
            await _outputCacheStore.EvictByTagAsync("home", default);
            TempData["Success"] = "Ad has been hidden.";
        }
        else
        {
            TempData["Error"] = "Failed to hide ad.";
        }
        return RedirectToAction("Ads");
    }

    public async Task<IActionResult> ShowAd(Guid id)
    {
        var result = await _adminService.ShowAdAsync(id);
        if (result)
        {
            await _outputCacheStore.EvictByTagAsync("home", default);
            TempData["Success"] = "Ad is now visible.";
        }
        else
        {
            TempData["Error"] = "Failed to show ad.";
        }
        return RedirectToAction("Ads");
    }

    public async Task<IActionResult> DeleteAd(Guid id)
    {
        var result = await _adminService.DeleteAdAsync(id);
        if (result)
        {
            await _outputCacheStore.EvictByTagAsync("home", default);
            TempData["Success"] = "Ad has been deleted.";
        }
        else
        {
            TempData["Error"] = "Failed to delete ad.";
        }
        return RedirectToAction("Ads");
    }
}
