using EIIOS.Models;

namespace EIIOS.ViewModels
{
    public class TimeEntryViewModel
    {
        public UserModel User { get; set; } = null!;
        public TimeEntryModel? ActiveTimeEntry { get; set; }
        public List<TimeEntryModel> TodaysEntries { get; set; } = new();
        public List<TimeEntryModel> RecentEntries { get; set; } = new();
        public decimal TodayTotalHours { get; set; }
        public decimal WeeklyTotalHours { get; set; }
        public decimal MonthlyTotalHours { get; set; }
        public bool IsClockedIn { get; set; }
    }
}