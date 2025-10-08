using EIIOS.Models;

namespace EIIOS.ViewModels
{
    public class TimeEntryHistoryViewModel
    {
        public List<TimeEntryModel> TimeEntries { get; set; } = new();
        public TimeEntryFilterViewModel Filter { get; set; } = new();
    }
}