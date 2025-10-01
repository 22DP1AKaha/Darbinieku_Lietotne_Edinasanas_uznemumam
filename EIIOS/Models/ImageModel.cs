using System.ComponentModel.DataAnnotations;

namespace EIIOS.Models
{
    public class ImageModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string FileName { get; set; } = string.Empty;

        [Required]
        public string Base64Data { get; set; } = string.Empty;

        [MaxLength(50)]
        public string ContentType { get; set; } = "image/jpeg"; // e.g., image/jpeg, image/png

        public long FileSize { get; set; } // Size in bytes

        [MaxLength(500)]
        public string? AltText { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property - only for products
        public ICollection<ProductModel>? Products { get; set; }
    }
}