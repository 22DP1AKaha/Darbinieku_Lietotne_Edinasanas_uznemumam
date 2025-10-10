using System.ComponentModel.DataAnnotations;

namespace EIIOS.ViewModels
{
    public class TimeEntryEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Clock in time is required")]
        public DateTime ClockIn { get; set; }

        public DateTime? ClockOut { get; set; }

        [Required(ErrorMessage = "Date is required")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }
    }
}