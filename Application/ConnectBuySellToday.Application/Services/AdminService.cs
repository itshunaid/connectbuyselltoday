using ConnectBuySellToday.Application.DTOs;
using ConnectBuySellToday.Application.Interfaces;
using ConnectBuySellToday.Domain.Entities;
using ConnectBuySellToday.Domain.Enums;
using ConnectBuySellToday.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ConnectBuySellToday.Application.Services;

public class AdminService : IAdminService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMessageService _messageService;

    public AdminService(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork, IMessageService messageService)
    {
        _userManager = userManager;
        _unitOfWork = unitOfWork;
        _messageService = messageService;
    }

    // User Management
    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var users = await _userManager.Users.ToListAsync();
        var userDtos = new List<UserDto>();

        foreach (var user in users)
        {
            var adCount = await _unitOfWork.Ads.GetAdsBySellerIdAsync(user.Id).ContinueWith(t => t.Result.Count());
            userDtos.Add(new UserDto
            {
                Id = user.Id,
                Email = user.Email ?? string.Empty,
                UserName = user.UserName ?? string.Empty,
                FirstName = user.FirstName,
                LastName = user.LastName,
                IsBanned = user.IsBanned,
                BanReason = user.BanReason,
                BanExpiresAt = user.BanExpiresAt,
                CreatedAt = user.CreatedAt,
                TotalAds = adCount
            });
        }

        return userDtos;
    }

    public async Task<UserDto?> GetUserByIdAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return null;

        var adCount = await _unitOfWork.Ads.GetAdsBySellerIdAsync(userId).ContinueWith(t => t.Result.Count());

        return new UserDto
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            UserName = user.UserName ?? string.Empty,
            FirstName = user.FirstName,
            LastName = user.LastName,
            IsBanned = user.IsBanned,
            BanReason = user.BanReason,
            BanExpiresAt = user.BanExpiresAt,
            CreatedAt = user.CreatedAt,
            TotalAds = adCount
        };
    }

    public async Task<bool> BanUserAsync(string userId, string reason, DateTime? banExpiresAt)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;

        user.IsBanned = true;
        user.BanReason = reason;
        user.BanExpiresAt = banExpiresAt;

        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<bool> UnbanUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;

        user.IsBanned = false;
        user.BanReason = null;
        user.BanExpiresAt = null;

        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    // Ad Management
    public async Task<IEnumerable<AdDto>> GetPendingAdsAsync()
    {
        var ads = await _unitOfWork.Ads.GetPendingAdsAsync();
        return await MapAdsToDto(ads);
    }

    public async Task<IEnumerable<AdDto>> GetAllAdsAsync(AdStatus? status = null)
    {
        var ads = await _unitOfWork.Ads.GetAllAdsAsync();
        
        if (status.HasValue)
        {
            ads = ads.Where(a => a.Status == status.Value);
        }

        return await MapAdsToDto(ads);
    }

    public async Task<bool> ApproveAdAsync(Guid adId)
    {
        var ad = await _unitOfWork.Ads.GetByIdAsync(adId);
        if (ad == null) return false;

        ad.Status = AdStatus.Active;
        _unitOfWork.Ads.Update(ad);
        var result = await _unitOfWork.CompleteAsync();
        return result > 0;
    }

    public async Task<bool> RejectAdAsync(Guid adId, string reason)
    {
        var ad = await _unitOfWork.Ads.GetByIdAsync(adId);
        if (ad == null) return false;

        ad.Status = AdStatus.Rejected;
        _unitOfWork.Ads.Update(ad);
        
        // Send rejection message to seller via MessageService
        var adminUserId = "system"; // Admin user ID
        var rejectionMessage = $"Your ad '{ad.Title}' has been rejected. Reason: {reason}";
        await _messageService.SendMessageAsync(adminUserId, ad.SellerId, adId, rejectionMessage);
        
        var result = await _unitOfWork.CompleteAsync();
        return result > 0;
    }

    public async Task<bool> HideAdAsync(Guid adId)
    {
        var ad = await _unitOfWork.Ads.GetByIdAsync(adId);
        if (ad == null) return false;

        ad.Status = AdStatus.Hidden;
        _unitOfWork.Ads.Update(ad);
        var result = await _unitOfWork.CompleteAsync();
        return result > 0;
    }

    public async Task<bool> ShowAdAsync(Guid adId)
    {
        var ad = await _unitOfWork.Ads.GetByIdAsync(adId);
        if (ad == null) return false;

        ad.Status = AdStatus.Active;
        _unitOfWork.Ads.Update(ad);
        var result = await _unitOfWork.CompleteAsync();
        return result > 0;
    }

    public async Task<bool> DeleteAdAsync(Guid adId)
    {
        var ad = await _unitOfWork.Ads.GetByIdAsync(adId);
        if (ad == null) return false;

        _unitOfWork.Ads.Delete(ad);
        var result = await _unitOfWork.CompleteAsync();
        return result > 0;
    }

    // Statistics
    public async Task<AdminDashboardDto> GetDashboardStatsAsync()
    {
        var totalUsers = await _userManager.Users.CountAsync();
        var bannedUsers = await _userManager.Users.CountAsync(u => u.IsBanned);
        
        return new AdminDashboardDto
        {
            TotalUsers = totalUsers,
            ActiveUsers = totalUsers - bannedUsers,
            BannedUsers = bannedUsers,
            TotalAds = await _unitOfWork.Ads.GetTotalAdsCountAsync(),
            ActiveAds = await _unitOfWork.Ads.GetActiveAdsCountAsync(),
            PendingAds = await _unitOfWork.Ads.GetPendingAdsCountAsync(),
            SoldAds = await _unitOfWork.Ads.GetSoldAdsCountAsync()
        };
    }

    private async Task<IEnumerable<AdDto>> MapAdsToDto(IEnumerable<ProductAd> ads)
    {
        var adList = ads.ToList();
        var sellerIds = adList.Select(a => a.SellerId).Distinct().ToList();
        var sellers = new Dictionary<string, ApplicationUser>();
        
        foreach (var sellerId in sellerIds)
        {
            var seller = await _userManager.FindByIdAsync(sellerId);
            if (seller != null)
            {
                sellers[sellerId] = seller;
            }
        }
        
        return adList.Select(a => new AdDto
        {
            Id = a.Id,
            Title = a.Title,
            Description = a.Description,
            Price = a.Price,
            Status = a.Status,
            CategoryId = a.CategoryId,
            CategoryName = a.Category?.Name ?? "General",
            SellerId = a.SellerId,
            SellerName = sellers.TryGetValue(a.SellerId, out var seller) 
                ? $"{seller.FirstName} {seller.LastName}" 
                : "Unknown",
            MainImageUrl = a.Images.FirstOrDefault(i => i.IsMain)?.Url ?? a.Images.FirstOrDefault()?.Url,
            ImageUrls = a.Images.Select(i => i.Url).ToList(),
            CreatedAt = a.CreatedAt
        });
    }
}
