using EIIOS.Models;
using System.ComponentModel.DataAnnotations;

namespace EIIOS.ViewModels
{
    public class TimeEntryFilterViewModel
    {
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        public int? EmployeeId { get; set; }

        public TimeEntryStatus? Status { get; set; }
    }
}