using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EIIOS.Models
{
    public class ProductAllergenModel
    {
        [Key, Column(Order = 0)]
        [ForeignKey("Product")]
        public int ProductId { get; set; }

        [Key, Column(Order = 1)]
        [ForeignKey("Allergen")]
        public int AllergenId { get; set; }

        // Navigation Properties
        public virtual ProductModel Product { get; set; } = null!;
        public virtual AllergenModel Allergen { get; set; } = null!;
    }
}
