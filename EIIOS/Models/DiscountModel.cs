using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EIIOS.Models
{
    public enum DiscountType
    {
        Percentage,
        FixedAmount
    }

    public class DiscountModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        public DiscountType DiscountType { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal DiscountValue { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("CreatedBy")]
        public int? CreatedById { get; set; }

        // Navigation Properties
        public virtual UserModel? CreatedBy { get; set; }
        public virtual ICollection<DiscountProductModel>? DiscountProducts { get; set; }

        [NotMapped]
        public bool IsCurrentlyActive => IsActive &&
            StartDate <= DateTime.Today &&
            (!EndDate.HasValue || EndDate.Value >= DateTime.Today);
    }
}
