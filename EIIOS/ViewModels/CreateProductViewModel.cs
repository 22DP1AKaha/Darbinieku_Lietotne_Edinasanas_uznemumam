using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace EIIOS.ViewModels
{
    public class CreateProductViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources.ViewModels.ProductViewModel),
                  ErrorMessageResourceName = "NameRequired")]
        [StringLength(100, MinimumLength = 2, ErrorMessageResourceType = typeof(Resources.ViewModels.ProductViewModel),
                      ErrorMessageResourceName = "NameLength")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessageResourceType = typeof(Resources.ViewModels.ProductViewModel),
                      ErrorMessageResourceName = "DescriptionLength")]
        public string? Description { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.ViewModels.ProductViewModel),
                  ErrorMessageResourceName = "PriceRequired")]
        [Range(0.01, 10000, ErrorMessageResourceType = typeof(Resources.ViewModels.ProductViewModel),
               ErrorMessageResourceName = "PriceRange")]
        public decimal BasePrice { get; set; }

        [Range(1, 300, ErrorMessageResourceType = typeof(Resources.ViewModels.ProductViewModel),
               ErrorMessageResourceName = "PrepTimeRange")]
        public int? PreparationTime { get; set; }

        public bool IsAvailable { get; set; } = true;
        public bool IsActive { get; set; } = true;

        public IFormFile? ImageFile { get; set; }
        public string? ImageAltText { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.ViewModels.ProductViewModel),
                  ErrorMessageResourceName = "CategoryRequired")]
        [MinLength(1, ErrorMessageResourceType = typeof(Resources.ViewModels.ProductViewModel),
                   ErrorMessageResourceName = "CategoryRequired")]
        public List<int> SelectedCategoryIds { get; set; } = new List<int>();

        public List<int> SelectedAllergenIds { get; set; } = new List<int>();

        // NEW: Ingredients
        public List<ProductIngredientViewModel> Ingredients { get; set; } = new List<ProductIngredientViewModel>();
    }

    public class ProductIngredientViewModel
    {
        public int InventoryItemId { get; set; }
        public decimal QuantityNeeded { get; set; }
        public string Unit { get; set; } = string.Empty;
    }
}