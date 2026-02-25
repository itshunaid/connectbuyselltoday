using ConnectBuySellToday.Application.DTOs;

namespace ConnectBuySellToday.Application.Interfaces;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
    Task<IEnumerable<CategoryDto>> GetRootCategoriesAsync();
    Task<CategoryDto?> GetCategoryByIdAsync(Guid id);
}
