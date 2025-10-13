using EIIOS.Models; // Add this using directive
using System.Collections.Generic;

namespace EIIOS.ViewModels
{
    public class TimeEntryAdminViewModel
    {
        public List<TimeEntryModel> TimeEntries { get; set; } = new();
        public TimeEntryFilterViewModel Filter { get; set; } = new();
        public int PendingCount { get; set; }
    }
}