using Microsoft.AspNetCore.Identity;

namespace ConnectBuySellToday.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public bool IsBanned { get; set; } = false;
    public string? BanReason { get; set; }
    public DateTime? BanExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
