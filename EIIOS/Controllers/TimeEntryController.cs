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

        // GET: TimeEntry/Admin - Admin view of all time entries
        [HttpGet]
        public async Task<IActionResult> Admin(TimeEntryFilterViewModel? filter = null)
        {
            try
            {
                var currentUserId = HttpContext.Session.GetInt32("UserId");
                var currentUserRole = HttpContext.Session.GetString("UserRole");

                if (currentUserId == null || currentUserRole != UserRole.Administrator.ToString())
                {
                    TempData["ErrorMessage"] = _localizer["AdminAccessRequired"].Value;
                    return RedirectToAction("Index", "Home");
                }

                IQueryable<TimeEntryModel> query = _context.TimeEntries
                    .Include(te => te.Employee)
                    .Include(te => te.ApprovedBy)
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

                    if (filter.EmployeeId.HasValue)
                    {
                        query = query.Where(te => te.EmployeeId == filter.EmployeeId.Value);
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

                // Get employees for filter dropdown
                ViewBag.Employees = await _context.Users
                    .Where(u => u.Role == UserRole.Employee && u.IsActive)
                    .OrderBy(u => u.FirstName)
                    .ThenBy(u => u.LastName)
                    .ToListAsync();

                var viewModel = new TimeEntryAdminViewModel
                {
                    TimeEntries = timeEntries,
                    Filter = filter ?? new TimeEntryFilterViewModel(),
                    PendingCount = timeEntries.Count(te => te.Status == TimeEntryStatus.Pending)
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading admin time entries for user {UserId}",
                    HttpContext.Session.GetInt32("UserId"));
                TempData["ErrorMessage"] = _localizer["ErrorLoadingAdminView"].Value;
                return View(new TimeEntryAdminViewModel());
            }
        }

        // POST: TimeEntry/Approve/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            try
            {
                var currentUserId = HttpContext.Session.GetInt32("UserId");
                var currentUserRole = HttpContext.Session.GetString("UserRole");

                if (currentUserId == null || currentUserRole != UserRole.Administrator.ToString())
                {
                    TempData["ErrorMessage"] = _localizer["AdminAccessRequired"].Value;
                    return RedirectToAction(nameof(Index));
                }

                var timeEntry = await _context.TimeEntries
                    .Include(te => te.Employee)
                    .FirstOrDefaultAsync(te => te.Id == id);

                if (timeEntry == null)
                {
                    TempData["ErrorMessage"] = _localizer["TimeEntryNotFound"].Value;
                    return RedirectToAction(nameof(Admin));
                }

                timeEntry.Status = TimeEntryStatus.Approved;
                timeEntry.ApprovedById = currentUserId;
                timeEntry.ApprovedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Time entry {TimeEntryId} approved by admin {AdminId}", id, currentUserId);

                TempData["SuccessMessage"] = _localizer["TimeEntryApproved"].Value;
                return RedirectToAction(nameof(Admin));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving time entry {TimeEntryId}", id);
                TempData["ErrorMessage"] = _localizer["ApprovalError"].Value;
                return RedirectToAction(nameof(Admin));
            }
        }

        // POST: TimeEntry/Reject/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id, string? rejectionReason)
        {
            try
            {
                var currentUserId = HttpContext.Session.GetInt32("UserId");
                var currentUserRole = HttpContext.Session.GetString("UserRole");

                if (currentUserId == null || currentUserRole != UserRole.Administrator.ToString())
                {
                    TempData["ErrorMessage"] = _localizer["AdminAccessRequired"].Value;
                    return RedirectToAction(nameof(Index));
                }

                var timeEntry = await _context.TimeEntries
                    .Include(te => te.Employee)
                    .FirstOrDefaultAsync(te => te.Id == id);

                if (timeEntry == null)
                {
                    TempData["ErrorMessage"] = _localizer["TimeEntryNotFound"].Value;
                    return RedirectToAction(nameof(Admin));
                }

                timeEntry.Status = TimeEntryStatus.Rejected;
                timeEntry.ApprovedById = currentUserId;
                timeEntry.ApprovedAt = DateTime.UtcNow;

                // Add rejection reason to notes
                if (!string.IsNullOrEmpty(rejectionReason))
                {
                    timeEntry.Notes = string.IsNullOrEmpty(timeEntry.Notes)
                        ? $"Rejection Reason: {rejectionReason}"
                        : $"{timeEntry.Notes}\nRejection Reason: {rejectionReason}";
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Time entry {TimeEntryId} rejected by admin {AdminId}", id, currentUserId);

                TempData["SuccessMessage"] = _localizer["TimeEntryRejected"].Value;
                return RedirectToAction(nameof(Admin));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting time entry {TimeEntryId}", id);
                TempData["ErrorMessage"] = _localizer["RejectionError"].Value;
                return RedirectToAction(nameof(Admin));
            }
        }

        // POST: TimeEntry/BulkApprove
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkApprove(int[] timeEntryIds)
        {
            try
            {
                var currentUserId = HttpContext.Session.GetInt32("UserId");
                var currentUserRole = HttpContext.Session.GetString("UserRole");

                if (currentUserId == null || currentUserRole != UserRole.Administrator.ToString())
                {
                    return Json(new { success = false, message = _localizer["AdminAccessRequired"].Value });
                }

                if (timeEntryIds == null || timeEntryIds.Length == 0)
                {
                    return Json(new { success = false, message = _localizer["NoEntriesSelected"].Value });
                }

                var timeEntries = await _context.TimeEntries
                    .Where(te => timeEntryIds.Contains(te.Id))
                    .ToListAsync();

                foreach (var timeEntry in timeEntries)
                {
                    timeEntry.Status = TimeEntryStatus.Approved;
                    timeEntry.ApprovedById = currentUserId;
                    timeEntry.ApprovedAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Bulk approved {Count} time entries by admin {AdminId}", timeEntries.Count, currentUserId);

                return Json(new { success = true, message = _localizer["BulkApproveSuccess", timeEntries.Count].Value });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error bulk approving time entries");
                return Json(new { success = false, message = _localizer["BulkApproveError"].Value });
            }
        }

        // GET: TimeEntry/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var currentUserId = HttpContext.Session.GetInt32("UserId");
                var currentUserRole = HttpContext.Session.GetString("UserRole");

                if (currentUserId == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var timeEntry = await _context.TimeEntries
                    .Include(te => te.Employee)
                    .Include(te => te.ApprovedBy)
                    .FirstOrDefaultAsync(te => te.Id == id);

                if (timeEntry == null)
                {
                    TempData["ErrorMessage"] = _localizer["TimeEntryNotFound"].Value;
                    return RedirectToAction(nameof(Index));
                }

                // Check permissions
                if (currentUserRole != UserRole.Administrator.ToString() &&
                    timeEntry.EmployeeId != currentUserId.Value)
                {
                    TempData["ErrorMessage"] = _localizer["AccessDenied"].Value;
                    return RedirectToAction(nameof(Index));
                }

                return View(timeEntry);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading time entry details for ID {TimeEntryId}", id);
                TempData["ErrorMessage"] = _localizer["ErrorLoadingTimeEntry"].Value;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: TimeEntry/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var currentUserId = HttpContext.Session.GetInt32("UserId");
                var currentUserRole = HttpContext.Session.GetString("UserRole");

                if (currentUserId == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var timeEntry = await _context.TimeEntries
                    .Include(te => te.Employee)
                    .FirstOrDefaultAsync(te => te.Id == id);

                if (timeEntry == null)
                {
                    TempData["ErrorMessage"] = _localizer["TimeEntryNotFound"].Value;
                    return RedirectToAction(nameof(Index));
                }

                // Check permissions - only owner or admin can edit
                if (currentUserRole != UserRole.Administrator.ToString() &&
                    timeEntry.EmployeeId != currentUserId.Value)
                {
                    TempData["ErrorMessage"] = _localizer["AccessDenied"].Value;
                    return RedirectToAction(nameof(Index));
                }

                // Only pending entries can be edited
                if (timeEntry.Status != TimeEntryStatus.Pending)
                {
                    TempData["ErrorMessage"] = _localizer["CannotEditApprovedOrRejected"].Value;
                    return RedirectToAction(nameof(Index));
                }

                var viewModel = new TimeEntryEditViewModel
                {
                    Id = timeEntry.Id,
                    ClockIn = timeEntry.ClockIn,
                    ClockOut = timeEntry.ClockOut,
                    Date = timeEntry.Date,
                    Notes = timeEntry.Notes
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading time entry for edit ID {TimeEntryId}", id);
                TempData["ErrorMessage"] = _localizer["ErrorLoadingTimeEntry"].Value;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: TimeEntry/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TimeEntryEditViewModel model)
        {
            try
            {
                var currentUserId = HttpContext.Session.GetInt32("UserId");
                var currentUserRole = HttpContext.Session.GetString("UserRole");

                if (currentUserId == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var timeEntry = await _context.TimeEntries
                    .FirstOrDefaultAsync(te => te.Id == id);

                if (timeEntry == null)
                {
                    TempData["ErrorMessage"] = _localizer["TimeEntryNotFound"].Value;
                    return RedirectToAction(nameof(Index));
                }

                // Check permissions
                if (currentUserRole != UserRole.Administrator.ToString() &&
                    timeEntry.EmployeeId != currentUserId.Value)
                {
                    TempData["ErrorMessage"] = _localizer["AccessDenied"].Value;
                    return RedirectToAction(nameof(Index));
                }

                // Only pending entries can be edited
                if (timeEntry.Status != TimeEntryStatus.Pending)
                {
                    TempData["ErrorMessage"] = _localizer["CannotEditApprovedOrRejected"].Value;
                    return RedirectToAction(nameof(Index));
                }

                // Validate clock out is after clock in
                if (model.ClockOut.HasValue && model.ClockOut <= model.ClockIn)
                {
                    ModelState.AddModelError("ClockOut", _localizer["ClockOutMustBeAfterClockIn"].Value);
                    return View(model);
                }

                timeEntry.ClockIn = model.ClockIn;
                timeEntry.ClockOut = model.ClockOut;
                timeEntry.Date = model.ClockIn.Date;
                timeEntry.Notes = model.Notes;

                // Recalculate total hours if clock out is set
                if (timeEntry.ClockOut.HasValue)
                {
                    var duration = timeEntry.ClockOut.Value - timeEntry.ClockIn;
                    timeEntry.TotalHours = (decimal)Math.Round(duration.TotalHours, 2);
                }
                else
                {
                    timeEntry.TotalHours = null;
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Time entry {TimeEntryId} updated by user {UserId}", id, currentUserId);

                TempData["SuccessMessage"] = _localizer["TimeEntryUpdated"].Value;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating time entry {TimeEntryId}", id);
                TempData["ErrorMessage"] = _localizer["TimeEntryUpdateError"].Value;
                return View(model);
            }
        }

        // POST: TimeEntry/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var currentUserId = HttpContext.Session.GetInt32("UserId");
                var currentUserRole = HttpContext.Session.GetString("UserRole");

                if (currentUserId == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var timeEntry = await _context.TimeEntries
                    .FirstOrDefaultAsync(te => te.Id == id);

                if (timeEntry == null)
                {
                    TempData["ErrorMessage"] = _localizer["TimeEntryNotFound"].Value;
                    return RedirectToAction(nameof(Index));
                }

                // Check permissions - only owner or admin can delete
                if (currentUserRole != UserRole.Administrator.ToString() &&
                    timeEntry.EmployeeId != currentUserId.Value)
                {
                    TempData["ErrorMessage"] = _localizer["AccessDenied"].Value;
                    return RedirectToAction(nameof(Index));
                }

                // Only pending entries can be deleted
                if (timeEntry.Status != TimeEntryStatus.Pending)
                {
                    TempData["ErrorMessage"] = _localizer["CannotDeleteApprovedOrRejected"].Value;
                    return RedirectToAction(nameof(Index));
                }

                _context.TimeEntries.Remove(timeEntry);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Time entry {TimeEntryId} deleted by user {UserId}", id, currentUserId);

                TempData["SuccessMessage"] = _localizer["TimeEntryDeleted"].Value;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting time entry {TimeEntryId}", id);
                TempData["ErrorMessage"] = _localizer["DeleteError"].Value;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: TimeEntry/Summary
        [HttpGet]
        public async Task<IActionResult> Summary(TimeEntrySummaryViewModel? filter = null)
        {
            try
            {
                var currentUserId = HttpContext.Session.GetInt32("UserId");
                var currentUserRole = HttpContext.Session.GetString("UserRole");

                if (currentUserId == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                IQueryable<TimeEntryModel> query = _context.TimeEntries
                    .Include(te => te.Employee)
                    .Where(te => te.Status == TimeEntryStatus.Approved && te.TotalHours.HasValue);

                // Filter by current user if they are an employee
                if (currentUserRole == UserRole.Employee.ToString())
                {
                    query = query.Where(te => te.EmployeeId == currentUserId.Value);
                }

                // Apply date filter
                var startDate = filter?.StartDate ?? new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                var endDate = filter?.EndDate ?? startDate.AddMonths(1).AddDays(-1);

                query = query.Where(te => te.Date >= startDate && te.Date <= endDate);

                // Get employees for filter dropdown (admins only)
                if (currentUserRole == UserRole.Administrator.ToString())
                {
                    ViewBag.Employees = await _context.Users
                        .Where(u => u.Role == UserRole.Employee && u.IsActive)
                        .OrderBy(u => u.FirstName)
                        .ThenBy(u => u.LastName)
                        .ToListAsync();

                    if (filter?.EmployeeId.HasValue == true)
                    {
                        query = query.Where(te => te.EmployeeId == filter.EmployeeId.Value);
                    }
                }

                var timeEntries = await query.ToListAsync();

                // Group by employee and calculate totals
                var summary = timeEntries
     .GroupBy(te => te.Employee)
     .Select(g => new EmployeeTimeSummary
     {
         Employee = g.Key,
         TotalHours = g.Sum(te => te.TotalHours ?? 0),
         EntryCount = g.Count(),
         AverageHoursPerDay = g.GroupBy(te => te.Date)
                             .Average(group => group.Sum(te => te.TotalHours ?? 0))
     })
     .OrderBy(s => s.Employee.FirstName)
     .ThenBy(s => s.Employee.LastName)
     .ToList();

                var viewModel = new TimeEntrySummaryViewModel
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    EmployeeId = filter?.EmployeeId,
                    Summary = summary,
                    TotalHours = summary.Sum(s => s.TotalHours),
                    TotalEntries = summary.Sum(s => s.EntryCount)
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading time entry summary");
                TempData["ErrorMessage"] = _localizer["ErrorLoadingSummary"].Value;
                return View(new TimeEntrySummaryViewModel());
            }
        }

        // Helper method to format time span for display
        private string FormatTimeSpan(TimeSpan timeSpan)
        {
            return $"{(int)timeSpan.TotalHours}:{timeSpan.Minutes:00}";
        }
    }
}