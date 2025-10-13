using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EIIOS.Models
{
    public class DailyMenuItemModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("DailyMenu")]
        public int DailyMenuId { get; set; }

        [Required]
        [ForeignKey("Product")]
        public int ProductId { get; set; }

        public int DisplayOrder { get; set; } = 0;

        public bool IsAvailable { get; set; } = true;

        [Column(TypeName = "decimal(10,2)")]
        public decimal? SpecialPrice { get; set; }

        // Navigation Properties
        public virtual DailyMenuModel DailyMenu { get; set; } = null!;
        public virtual ProductModel Product { get; set; } = null!;

        [NotMapped]
        public decimal EffectivePrice => SpecialPrice ?? Product?.BasePrice ?? 0;
    }
}
