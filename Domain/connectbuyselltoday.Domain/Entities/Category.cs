using connectbuyselltoday.Domain.Common;

namespace connectbuyselltoday.Domain.Entities
{
   

    public class Category : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? IconUrl { get; set; }

        public Guid? ParentCategoryId { get; set; }
        public Category? ParentCategory { get; set; }

        public List<Category> SubCategories { get; set; } = new();
        public List<ProductAd> ProductAds { get; set; } = new();
    }
}
