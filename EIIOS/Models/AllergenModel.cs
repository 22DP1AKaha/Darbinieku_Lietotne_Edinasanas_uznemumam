using System.ComponentModel.DataAnnotations;

namespace EIIOS.Models
{
    public class AllergenModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [StringLength(10)]
        public string Code { get; set; }

        [StringLength(200)]
        public string? Description { get; set; }

        // Navigation Properties
        public virtual ICollection<ProductAllergenModel>? ProductAllergens { get; set; }
    }
}
