using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EIIOS.Models
{
    public class InventoryItemModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(20)]
        public string Unit { get; set; } // kg, l, pieces, etc.

        [Column(TypeName = "decimal(10,3)")]
        public decimal CurrentQuantity { get; set; } = 0;

        [Column(TypeName = "decimal(10,3)")]
        public decimal MinimumQuantity { get; set; } = 0;

        [Column(TypeName = "decimal(10,2)")]
        public decimal? UnitCost { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual ICollection<StockEntryModel>? StockEntries { get; set; }
        public virtual ICollection<ProductIngredientModel>? ProductIngredients { get; set; }

        [NotMapped]
        public bool IsLowStock => CurrentQuantity <= MinimumQuantity;
    }
}
