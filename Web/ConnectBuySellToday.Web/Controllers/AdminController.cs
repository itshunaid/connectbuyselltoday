using ConnectBuySellToday.Application.DTOs;
using ConnectBuySellToday.Application.Interfaces;
using ConnectBuySellToday.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

using IReportService = ConnectBuySellToday.Application.Interfaces.IReportService;

namespace ConnectBuySellToday.Web.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly IAdminService _adminService;
    private readonly IReportService _reportService;
    private readonly ILogger<AdminController> _logger;
    private readonly IOutputCacheStore _outputCacheStore;
    private readonly IMemoryCache _memoryCache;

    private const string LatestAdsCacheKey = "LatestAdsCache";

    public AdminController(
        IAdminService adminService,
        IReportService reportService,
        ILogger<AdminController> logger, 
        IOutputCacheStore outputCacheStore,
        IMemoryCache memoryCache)
    {
        _adminService = adminService;
        _reportService = reportService;
        _logger = logger;
        _outputCacheStore = outputCacheStore;
        _memoryCache = memoryCache;
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

    public async Task<IActionResult> ModerationQueue()
    {
        var ads = await _adminService.GetModerationQueueAsync();
        return View(ads);
    }

    // AJAX endpoint for approving ads
    [HttpGet]
    public async Task<IActionResult> ApproveAdAjax(Guid id)
    {
        var result = await _adminService.ApproveAdAsync(id);
        if (result)
        {
            await _outputCacheStore.EvictByTagAsync("home", default);
            // Invalidate IMemoryCache for latest ads
            _memoryCache.Remove(LatestAdsCacheKey);
            return Json(new { success = true, message = "Ad has been approved." });
        }
        return Json(new { success = false, message = "Failed to approve ad." });
    }

    // AJAX endpoint for rejecting ads
    [HttpPost]
    public async Task<IActionResult> RejectAdAjax(Guid id, string reason)
    {
        var result = await _adminService.RejectAdAsync(id, reason);
        if (result)
        {
            await _outputCacheStore.EvictByTagAsync("home", default);
            return Json(new { success = true, message = "Ad has been rejected." });
        }
        return Json(new { success = false, message = "Failed to reject ad." });
    }

    // Legacy endpoints for non-AJAX requests
    public async Task<IActionResult> ApproveAd(Guid id)
    {
        var result = await _adminService.ApproveAdAsync(id);
        if (result)
        {
            await _outputCacheStore.EvictByTagAsync("home", default);
            // Invalidate IMemoryCache for latest ads
            _memoryCache.Remove(LatestAdsCacheKey);
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

    // Report Management
    public async Task<IActionResult> Reports(ReportStatus? status)
    {
        var reports = await _reportService.GetAllReportsAsync(status);
        return View(reports);
    }

    public async Task<IActionResult> PendingReports()
    {
        var reports = await _reportService.GetPendingReportsAsync();
        return View(reports);
    }

    public async Task<IActionResult> ReportDetails(Guid id)
    {
        var report = await _reportService.GetReportByIdAsync(id);
        if (report == null)
        {
            return NotFound();
        }
        return View(report);
    }

    [HttpPost]
    public async Task<IActionResult> ResolveReport(Guid id, ReportStatus status, string? adminNotes)
    {
        var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(adminId))
        {
            TempData["Error"] = "Unable to identify admin user.";
            return RedirectToAction("Reports");
        }

        var resolveDto = new ResolveReportDto
        {
            Status = status,
            AdminNotes = adminNotes
        };

        var result = await _reportService.ResolveReportAsync(id, adminId, resolveDto);
        if (result)
        {
            TempData["Success"] = "Report has been resolved.";
        }
        else
        {
            TempData["Error"] = "Failed to resolve report.";
        }
        return RedirectToAction("Reports");
    }

    [HttpPost]
    public async Task<IActionResult> DeleteReport(Guid id)
    {
        // We'll implement this by getting the report and deleting it
        var report = await _reportService.GetReportByIdAsync(id);
        if (report == null)
        {
            TempData["Error"] = "Report not found.";
            return RedirectToAction("Reports");
        }

        // For now, we can resolve it as Reviewed without action
        var resolveDto = new ResolveReportDto
        {
            Status = ReportStatus.Reviewed,
            AdminNotes = "Report dismissed - no action needed"
        };

        var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!string.IsNullOrEmpty(adminId))
        {
            await _reportService.ResolveReportAsync(id, adminId, resolveDto);
            TempData["Success"] = "Report has been dismissed.";
        }
        else
        {
            TempData["Error"] = "Unable to identify admin user.";
        }
        
        return RedirectToAction("Reports");
    }
}
