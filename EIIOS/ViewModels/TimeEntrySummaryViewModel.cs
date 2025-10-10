using EIIOS.Models;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace EIIOS.ViewModels
{
    public class TimeEntrySummaryViewModel
    {
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        public int? EmployeeId { get; set; }

        public List<EmployeeTimeSummary> Summary { get; set; } = new();
        public decimal TotalHours { get; set; }
        public int TotalEntries { get; set; }
    }

    public class EmployeeTimeSummary
    {
        public UserModel Employee { get; set; } = null!;
        public decimal TotalHours { get; set; }
        public int EntryCount { get; set; }
        public decimal AverageHoursPerDay { get; set; } // Changed from double to decimal
    }
}