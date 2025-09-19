using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EIIOS.Models
{
    public enum UserRole
    {
        Client,
        Employee,
        Administrator
    }

    public class UserModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(255)]
        public string PasswordHash { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [Required]
        public UserRole Role { get; set; } = UserRole.Client;

        public bool IsEmailVerified { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        // Navigation Properties
        public virtual ICollection<TimeEntryModel>? TimeEntries { get; set; }
        public virtual ICollection<TimeEntryModel>? ApprovedTimeEntries { get; set; }
        public virtual ICollection<ProductModel>? CreatedProducts { get; set; }
        public virtual ICollection<DailyMenuModel>? CreatedDailyMenus { get; set; }
        public virtual ICollection<StockEntryModel>? CreatedStockEntries { get; set; }
        public virtual ICollection<DiscountModel>? CreatedDiscounts { get; set; }

        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";
    }
}
