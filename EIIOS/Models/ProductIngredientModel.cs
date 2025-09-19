using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EIIOS.Models
{
    public class ProductIngredientModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Product")]
        public int ProductId { get; set; }

        [Required]
        [ForeignKey("InventoryItem")]
        public int InventoryItemId { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,3)")]
        public decimal QuantityNeeded { get; set; }

        [Required]
        [StringLength(20)]
        public string Unit { get; set; }

        // Navigation Properties
        public virtual ProductModel Product { get; set; } = null!;
        public virtual InventoryItemModel InventoryItem { get; set; } = null!;
    }
}
