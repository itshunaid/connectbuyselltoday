using ConnectBuySellToday.Domain.Entities;

namespace ConnectBuySellToday.Domain.Interfaces;

public interface IUserRepository
{
    Task<IEnumerable<ApplicationUser>> GetAllUsersAsync();
    Task<ApplicationUser?> GetByIdAsync(string userId);
    Task<int> GetUserAdCountAsync(string userId);
}
