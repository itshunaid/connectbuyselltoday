using ConnectBuySellToday.Application.DTOs;
using ConnectBuySellToday.Application.Interfaces;
using ConnectBuySellToday.Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace ConnectBuySellToday.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMemoryCache _cache;
    private const string CategoriesCacheKey = "Categories";
    private const string RootCategoriesCacheKey = "RootCategories";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);

    public CategoryService(IUnitOfWork unitOfWork, IMemoryCache cache)
    {
        _unitOfWork = unitOfWork;
        _cache = cache;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
    {
        if (!_cache.TryGetValue(CategoriesCacheKey, out IEnumerable<CategoryDto>? categories))
        {
            var allCategories = await _unitOfWork.Categories.GetAllAsync();
            categories = allCategories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                IconUrl = c.IconUrl,
                ParentCategoryId = c.ParentCategoryId
            }).ToList();

            _cache.Set(CategoriesCacheKey, categories, CacheDuration);
        }

        return categories!;
    }

    public async Task<IEnumerable<CategoryDto>> GetRootCategoriesAsync()
    {
        if (!_cache.TryGetValue(RootCategoriesCacheKey, out IEnumerable<CategoryDto>? rootCategories))
        {
            var rootCategoriesList = await _unitOfWork.Categories.GetRootCategoriesAsync();
            rootCategories = rootCategoriesList.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                IconUrl = c.IconUrl,
                ParentCategoryId = c.ParentCategoryId
            }).ToList();

            _cache.Set(RootCategoriesCacheKey, rootCategories, CacheDuration);
        }

        return rootCategories!;
    }

    public async Task<CategoryDto?> GetCategoryByIdAsync(Guid id)
    {
        var categories = await GetAllCategoriesAsync();
        return categories.FirstOrDefault(c => c.Id == id);
    }

    public void InvalidateCache()
    {
        _cache.Remove(CategoriesCacheKey);
        _cache.Remove(RootCategoriesCacheKey);
    }
}
