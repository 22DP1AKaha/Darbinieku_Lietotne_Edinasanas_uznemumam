using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EIIOS.Models
{
    public class ProductModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal BasePrice { get; set; }

        [StringLength(255)]
        public string? ImageUrl { get; set; }

        public int? PreparationTime { get; set; } // in minutes

        public bool IsAvailable { get; set; } = true;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("CreatedBy")]
        public int? CreatedById { get; set; }

        // Navigation Properties
        public virtual UserModel? CreatedBy { get; set; }
        public virtual ICollection<ProductCategoryModel>? ProductCategories { get; set; }
        public virtual ICollection<ProductAllergenModel>? ProductAllergens { get; set; }
        public virtual ICollection<DailyMenuItemModel>? DailyMenuItems { get; set; }
        public virtual ICollection<ProductIngredientModel>? ProductIngredients { get; set; }
        public virtual ICollection<DiscountProductModel>? DiscountProducts { get; set; }
    }
}
