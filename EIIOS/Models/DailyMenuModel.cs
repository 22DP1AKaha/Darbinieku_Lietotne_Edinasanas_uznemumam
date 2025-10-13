using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EIIOS.Models
{
    public class DailyMenuModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime MenuDate { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("CreatedBy")]
        public int? CreatedById { get; set; }

        // Navigation Properties
        public virtual UserModel? CreatedBy { get; set; }
        public virtual ICollection<DailyMenuItemModel>? DailyMenuItems { get; set; }
    }
}
