using ConnectBuySellToday.Application.DTOs;
using ConnectBuySellToday.Domain.Enums;

namespace ConnectBuySellToday.Application.Interfaces;

public interface IAdminService
{
    // User Management
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task<UserDto?> GetUserByIdAsync(string userId);
    Task<bool> BanUserAsync(string userId, string reason, DateTime? banExpiresAt);
    Task<bool> UnbanUserAsync(string userId);

    // Ad Management
    Task<IEnumerable<AdDto>> GetPendingAdsAsync();
    Task<IEnumerable<AdDto>> GetAllAdsAsync(AdStatus? status = null);
    Task<IEnumerable<AdDto>> GetModerationQueueAsync();

    Task<bool> ApproveAdAsync(Guid adId);
    Task<bool> RejectAdAsync(Guid adId, string reason);
    Task<bool> HideAdAsync(Guid adId);
    Task<bool> ShowAdAsync(Guid adId);
    Task<bool> DeleteAdAsync(Guid adId);

    // Statistics
    Task<AdminDashboardDto> GetDashboardStatsAsync();
}

public class UserDto
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public bool IsBanned { get; set; }
    public string? BanReason { get; set; }
    public DateTime? BanExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public int TotalAds { get; set; }
}

public class AdminDashboardDto
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int BannedUsers { get; set; }
    public int TotalAds { get; set; }
    public int ActiveAds { get; set; }
    public int PendingAds { get; set; }
    public int SoldAds { get; set; }
}
