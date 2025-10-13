using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EIIOS.Models
{
    public class StockEntryModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("InventoryItem")]
        public int InventoryItemId { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,3)")]
        public decimal Quantity { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ExpiryDate { get; set; }

        [StringLength(50)]
        public string? BatchNumber { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime ReceivedDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("CreatedBy")]
        public int? CreatedById { get; set; }

        // Navigation Properties
        public virtual InventoryItemModel InventoryItem { get; set; } = null!;
        public virtual UserModel? CreatedBy { get; set; }

        [NotMapped]
        public bool IsExpiringSoon => ExpiryDate.HasValue && ExpiryDate.Value <= DateTime.Today.AddDays(3);

        [NotMapped]
        public bool IsExpired => ExpiryDate.HasValue && ExpiryDate.Value < DateTime.Today;
    }
}
