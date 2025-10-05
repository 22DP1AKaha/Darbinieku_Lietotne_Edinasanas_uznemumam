using System.ComponentModel.DataAnnotations;

namespace EIIOS.ViewModels
{
    public class CreateDiscountViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources.ViewModels.DiscountViewModel),
                  ErrorMessageResourceName = "NameRequired")]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.ViewModels.DiscountViewModel),
                  ErrorMessageResourceName = "TypeRequired")]
        public string DiscountType { get; set; } = "Percentage";

        [Required(ErrorMessageResourceType = typeof(Resources.ViewModels.DiscountViewModel),
                  ErrorMessageResourceName = "ValueRequired")]
        public decimal DiscountValue { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.ViewModels.DiscountViewModel),
                  ErrorMessageResourceName = "StartDateRequired")]
        public DateTime StartDate { get; set; } = DateTime.UtcNow;

        public DateTime? EndDate { get; set; }

        public bool IsActive { get; set; } = true;

        [Required(ErrorMessageResourceType = typeof(Resources.ViewModels.DiscountViewModel),
                  ErrorMessageResourceName = "ProductRequired")]
        public List<int> SelectedProductIds { get; set; } = new List<int>();
    }
}
