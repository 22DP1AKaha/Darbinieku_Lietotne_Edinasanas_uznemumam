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
    }
}