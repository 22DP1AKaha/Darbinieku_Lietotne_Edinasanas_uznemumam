using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EIIOS.Models
{
    public class DiscountProductModel
    {
        [Key, Column(Order = 0)]
        [ForeignKey("Discount")]
        public int DiscountId { get; set; }

        [Key, Column(Order = 1)]
        [ForeignKey("Product")]
        public int ProductId { get; set; }

        // Navigation Properties
        public virtual DiscountModel Discount { get; set; } = null!;
        public virtual ProductModel Product { get; set; } = null!;
    }
}
