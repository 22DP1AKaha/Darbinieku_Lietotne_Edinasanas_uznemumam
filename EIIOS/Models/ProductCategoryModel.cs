using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EIIOS.Models
{
    public class ProductCategoryModel
    {
        [Key, Column(Order = 0)]
        [ForeignKey("Product")]
        public int ProductId { get; set; }

        [Key, Column(Order = 1)]
        [ForeignKey("Category")]
        public int CategoryId { get; set; }

        // Navigation Properties
        public virtual ProductModel Product { get; set; } = null!;
        public virtual CategoryModel Category { get; set; } = null!;
    }
}
