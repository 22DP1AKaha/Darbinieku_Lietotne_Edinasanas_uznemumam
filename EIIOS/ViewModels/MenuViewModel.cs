using EIIOS.Models;

namespace EIIOS.ViewModels
{
    public class MenuViewModel
    {
        public List<ProductModel> Products { get; set; } = new List<ProductModel>();
        public List<CategoryModel> Categories { get; set; } = new List<CategoryModel>();
        public string SearchTerm { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string SortBy { get; set; } = "name";

        // Daily Menu properties
        public DailyMenuModel? TodayMenu { get; set; }
        public List<DailyMenuItemModel> TodayMenuItems { get; set; } = new List<DailyMenuItemModel>();
        public bool HasDailyMenu => TodayMenu != null && TodayMenuItems.Any();
    }
}