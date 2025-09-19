using System.ComponentModel.DataAnnotations;

namespace EIIOS.Models
{
    public class CategoryModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public int DisplayOrder { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        // Navigation Properties
        public virtual ICollection<ProductCategoryModel>? ProductCategories { get; set; }
    }
}
