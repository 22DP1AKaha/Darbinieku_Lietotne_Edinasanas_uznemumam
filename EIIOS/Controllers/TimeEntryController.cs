using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using EIIOS.Data;
using EIIOS.Models;
using EIIOS.ViewModels;
using EIIOS.Services;

namespace EIIOS.Controllers
{
    public class TimeEntryController : Controller
    {
        private readonly EIIOSDbContext _context;
        private readonly IStringLocalizer<TimeEntryController> _localizer;
        private readonly ILogger<TimeEntryController> _logger;

        public TimeEntryController(
            EIIOSDbContext context,
            IStringLocalizer<TimeEntryController> localizer,
            ILogger<TimeEntryController> logger)
        {
            _context = context;
            _localizer = localizer;
            _logger = logger;
        }

        // GET: TimeEntry/Index - Main time tracking dashboard
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var currentUserId = HttpContext.Session.GetInt32("UserId");
                if (currentUserId == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var user = await _context.Users.FindAsync(currentUserId);
                if (user == null)
                {
                    HttpContext.Session.Clear();
                    return RedirectToAction("Login", "Account");
                }

                // Get current active time entry (if clocked in)
                var activeEntry = await _context.TimeEntries
                    .FirstOrDefaultAsync(te =>
                        te.EmployeeId == currentUserId &&
                        !te.ClockOut.HasValue);

                // Get today's entries
                var today = DateTime.Today;
                var todaysEntries = await _context.TimeEntries
                    .Where(te => te.EmployeeId == currentUserId && te.Date == today)
                    .OrderBy(te => te.ClockIn)
                    .ToListAsync();

                // Calculate today's total hours
                var todayTotalHours = todaysEntries
                    .Where(te => te.TotalHours.HasValue)
                    .Sum(te => te.TotalHours.Value);

                // Calculate weekly total hours (current week)
                var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
                var endOfWeek = startOfWeek.AddDays(7).AddSeconds(-1);
                var weeklyEntries = await _context.TimeEntries
                    .Where(te => te.EmployeeId == currentUserId &&
                           te.Date >= startOfWeek && te.Date <= endOfWeek &&
                           te.TotalHours.HasValue)
                    .ToListAsync();
                var weeklyTotalHours = weeklyEntries.Sum(te => te.TotalHours.Value);

                // Calculate monthly total hours (current month)
                var startOfMonth = new DateTime(today.Year, today.Month, 1);
                var endOfMonth = startOfMonth.AddMonths(1).AddSeconds(-1);
                var monthlyEntries = await _context.TimeEntries
                    .Where(te => te.EmployeeId == currentUserId &&
                           te.Date >= startOfMonth && te.Date <= endOfMonth &&
                           te.TotalHours.HasValue)
                    .ToListAsync();
                var monthlyTotalHours = monthlyEntries.Sum(te => te.TotalHours.Value);

                // Get recent time entries (last 7 days)
                var recentEntries = await _context.TimeEntries
                    .Where(te => te.EmployeeId == currentUserId &&
                           te.Date >= today.AddDays(-7))
                    .OrderByDescending(te => te.Date)
                    .ThenByDescending(te => te.ClockIn)
                    .Take(10)
                    .ToListAsync();

                var viewModel = new TimeEntryViewModel
                {
                    User = user,
                    ActiveTimeEntry = activeEntry,
                    TodaysEntries = todaysEntries,
                    RecentEntries = recentEntries,
                    TodayTotalHours = todayTotalHours,
                    WeeklyTotalHours = weeklyTotalHours,
                    MonthlyTotalHours = monthlyTotalHours,
                    IsClockedIn = activeEntry != null
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading time entry dashboard for user {UserId}",
                    HttpContext.Session.GetInt32("UserId"));
                TempData["ErrorMessage"] = _localizer["ErrorLoadingDashboard"].Value;
                return View(new TimeEntryViewModel());
            }
        }

        // POST: TimeEntry/ClockIn
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClockIn()
        {
            try
            {
                var currentUserId = HttpContext.Session.GetInt32("UserId");
                if (currentUserId == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                // Check if already clocked in
                var activeEntry = await _context.TimeEntries
                    .FirstOrDefaultAsync(te =>
                        te.EmployeeId == currentUserId &&
                        !te.ClockOut.HasValue);

                if (activeEntry != null)
                {
                    TempData["ErrorMessage"] = _localizer["AlreadyClockedIn"].Value;
                    return RedirectToAction(nameof(Index));
                }

                var timeEntry = new TimeEntryModel
                {
                    EmployeeId = currentUserId.Value,
                    ClockIn = DateTime.UtcNow,
                    Date = DateTime.UtcNow.Date,
                    Status = TimeEntryStatus.Pending,
                    CreatedAt = DateTime.UtcNow
                };

                _context.TimeEntries.Add(timeEntry);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User {UserId} clocked in at {ClockInTime}",
                    currentUserId, timeEntry.ClockIn);

                TempData["SuccessMessage"] = _localizer["ClockedInSuccess"].Value;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during clock in for user {UserId}",
                    HttpContext.Session.GetInt32("UserId"));
                TempData["ErrorMessage"] = _localizer["ClockInError"].Value;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: TimeEntry/ClockOut
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClockOut()
        {
            try
            {
                var currentUserId = HttpContext.Session.GetInt32("UserId");
                if (currentUserId == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var activeEntry = await _context.TimeEntries
                    .FirstOrDefaultAsync(te =>
                        te.EmployeeId == currentUserId &&
                        !te.ClockOut.HasValue);

                if (activeEntry == null)
                {
                    TempData["ErrorMessage"] = _localizer["NotClockedIn"].Value;
                    return RedirectToAction(nameof(Index));
                }

                activeEntry.ClockOut = DateTime.UtcNow;

                // Calculate total hours
                var duration = activeEntry.ClockOut.Value - activeEntry.ClockIn;
                activeEntry.TotalHours = (decimal)Math.Round(duration.TotalHours, 2);

                await _context.SaveChangesAsync();

                _logger.LogInformation("User {UserId} clocked out at {ClockOutTime}",
                    currentUserId, activeEntry.ClockOut);

                TempData["SuccessMessage"] = _localizer["ClockedOutSuccess"].Value;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during clock out for user {UserId}",
                    HttpContext.Session.GetInt32("UserId"));
                TempData["ErrorMessage"] = _localizer["ClockOutError"].Value;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: TimeEntry/History
        [HttpGet]
        public async Task<IActionResult> History(TimeEntryFilterViewModel? filter = null)
        {
            try
            {
                var currentUserId = HttpContext.Session.GetInt32("UserId");
                if (currentUserId == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                IQueryable<TimeEntryModel> query = _context.TimeEntries
                    .Where(te => te.EmployeeId == currentUserId)
                    .OrderByDescending(te => te.Date)
                    .ThenByDescending(te => te.ClockIn);

                // Apply filters
                if (filter != null)
                {
                    if (filter.StartDate.HasValue)
                    {
                        query = query.Where(te => te.Date >= filter.StartDate.Value);
                    }

                    if (filter.EndDate.HasValue)
                    {
                        query = query.Where(te => te.Date <= filter.EndDate.Value);
                    }

                    if (filter.Status.HasValue)
                    {
                        query = query.Where(te => te.Status == filter.Status.Value);
                    }
                }

                // Default to current month if no date filter
                if (!filter?.StartDate.HasValue == true && !filter?.EndDate.HasValue == true)
                {
                    var today = DateTime.Today;
                    var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
                    var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
                    query = query.Where(te => te.Date >= firstDayOfMonth && te.Date <= lastDayOfMonth);
                }

                var timeEntries = await query.ToListAsync();

                var viewModel = new TimeEntryHistoryViewModel
                {
                    TimeEntries = timeEntries,
                    Filter = filter ?? new TimeEntryFilterViewModel()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading time entry history for user {UserId}",
                    HttpContext.Session.GetInt32("UserId"));
                TempData["ErrorMessage"] = _localizer["ErrorLoadingHistory"].Value;
                return View(new TimeEntryHistoryViewModel());
            }
        }

        // Helper method to format time span for display
        private string FormatTimeSpan(TimeSpan timeSpan)
        {
            return $"{(int)timeSpan.TotalHours}:{timeSpan.Minutes:00}";
        }
    }
}