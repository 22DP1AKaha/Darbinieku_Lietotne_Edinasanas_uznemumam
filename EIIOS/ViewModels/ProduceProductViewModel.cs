using System.ComponentModel.DataAnnotations;

namespace EIIOS.ViewModels
{
    public class ProduceProductViewModel
    {
        [Required]
        public int ProductId { get; set; }

        [Display(Name = "Product Name")]
        public string ProductName { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(Resources.ViewModels.ProductViewModel),
                  ErrorMessageResourceName = "QuantityRequired")]
        [Range(0.01, 10000, ErrorMessageResourceType = typeof(Resources.ViewModels.ProductViewModel),
               ErrorMessageResourceName = "QuantityRange")]
        [Display(Name = "Quantity Produced")]
        public int QuantityProduced { get; set; }

        public List<IngredientDisplay>? Ingredients { get; set; }

        public class IngredientDisplay
        {
            public string Name { get; set; } = string.Empty;
            public decimal QuantityNeeded { get; set; }
            public string Unit { get; set; } = string.Empty;
        }
    }
}
