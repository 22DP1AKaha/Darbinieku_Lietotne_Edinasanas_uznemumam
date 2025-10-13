using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EIIOS.Models
{
    public enum TimeEntryStatus
    {
        Pending,
        Approved,
        Rejected
    }

    public class TimeEntryModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Employee")]
        public int EmployeeId { get; set; }

        [Required]
        public DateTime ClockIn { get; set; }

        public DateTime? ClockOut { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? TotalHours { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        public TimeEntryStatus Status { get; set; } = TimeEntryStatus.Pending;

        [StringLength(500)]
        public string? Notes { get; set; }

        [ForeignKey("ApprovedBy")]
        public int? ApprovedById { get; set; }

        public DateTime? ApprovedAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual UserModel Employee { get; set; } = null!;
        public virtual UserModel? ApprovedBy { get; set; }

        [NotMapped]
        public bool IsComplete => ClockOut.HasValue;

        [NotMapped]
        public TimeSpan? WorkDuration => ClockOut.HasValue ? ClockOut.Value - ClockIn : null;
    }
}
