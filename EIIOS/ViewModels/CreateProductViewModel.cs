using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace EIIOS.ViewModels
{
    public class CreateProductViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources.ViewModels.ProductViewModel),
                  ErrorMessageResourceName = "NameRequired")]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.ViewModels.ProductViewModel),
                  ErrorMessageResourceName = "PriceRequired")]
        public decimal BasePrice { get; set; }
        public int? PreparationTime { get; set; }

        public bool IsAvailable { get; set; } = true;

        public bool IsActive { get; set; } = true;

        public IFormFile? ImageFile { get; set; }

        public string? ImageAltText { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.ViewModels.ProductViewModel),
                  ErrorMessageResourceName = "CategoryRequired")]
        public List<int> SelectedCategoryIds { get; set; } = new List<int>();

        public List<int> SelectedAllergenIds { get; set; } = new List<int>();
    }
}