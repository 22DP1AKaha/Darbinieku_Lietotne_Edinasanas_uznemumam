using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using EIIOS.Data;
using EIIOS.Models;
using EIIOS.ViewModels;
using EIIOS.Services;
using BCrypt.Net;

namespace EIIOS.Controllers
{
    public class AdminController : Controller
    {
        private readonly EIIOSDbContext _context;
        private readonly IStringLocalizer<AdminController> _localizer;
        private readonly UserService _userService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(
            EIIOSDbContext context,
            IStringLocalizer<AdminController> localizer,
            UserService userService,
            ILogger<AdminController> logger)
        {
            _context = context;
            _localizer = localizer;
            _userService = userService;
            _logger = logger;
        }

        // GET: Admin/Employees
        [HttpGet]
        public async Task<IActionResult> Employees(string searchTerm = "", string roleFilter = "", string statusFilter = "")
        {
            if (!await _userService.IsCurrentUserAdmin())
            {
                TempData["ErrorMessage"] = _localizer["Unauthorized"].Value;
                return RedirectToAction("Index", "Home");
            }

            var query = _context.Users.AsQueryable();

            // Search filter
            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(u =>
                    u.Username.ToLower().Contains(searchTerm) ||
                    u.Email.ToLower().Contains(searchTerm) ||
                    u.FirstName.ToLower().Contains(searchTerm) ||
                    u.LastName.ToLower().Contains(searchTerm));
            }

            // Role filter
            if (!string.IsNullOrEmpty(roleFilter))
            {
                if (Enum.TryParse<UserRole>(roleFilter, out var role))
                {
                    query = query.Where(u => u.Role == role);
                }
            }

            // Status filter
            if (!string.IsNullOrEmpty(statusFilter))
            {
                bool isActive = statusFilter.ToLower() == "active";
                query = query.Where(u => u.IsActive == isActive);
            }

            var employees = await query
                .OrderByDescending(u => u.CreatedAt)
                .Select(u => new EmployeeViewModel
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Role = u.Role.ToString(),
                    IsActive = u.IsActive,
                    IsEmailVerified = u.IsEmailVerified,
                    CreatedAt = u.CreatedAt
                })
                .ToListAsync();

            var viewModel = new EmployeeListViewModel
            {
                Employees = employees,
                SearchTerm = searchTerm,
                RoleFilter = roleFilter,
                StatusFilter = statusFilter
            };

            return View(viewModel);
        }

        // GET: Admin/GetEmployee
        [HttpGet]
        public async Task<IActionResult> GetEmployee(int id)
        {
            if (!await _userService.IsCurrentUserAdmin())
                return Unauthorized();

            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            return Json(new
            {
                id = user.Id,
                username = user.Username,
                email = user.Email,
                firstName = user.FirstName,
                lastName = user.LastName,
                role = user.Role.ToString(),
                isActive = user.IsActive,
                isEmailVerified = user.IsEmailVerified
            });
        }

        // POST: Admin/CreateEmployee
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEmployee(EmployeeViewModel model)
        {
            if (!await _userService.IsCurrentUserAdmin())
            {
                return Json(new { success = false, errors = new { general = _localizer["Unauthorized"].Value } });
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );
                return Json(new { success = false, errors });
            }

            // Check if username exists
            if (await _context.Users.AnyAsync(u => u.Username.ToLower() == model.Username.ToLower()))
            {
                return Json(new { success = false, errors = new { Username = new[] { _localizer["UsernameExists"].Value } } });
            }

            // Check if email exists
            if (await _context.Users.AnyAsync(u => u.Email.ToLower() == model.Email.ToLower()))
            {
                return Json(new { success = false, errors = new { Email = new[] { _localizer["EmailExists"].Value } } });
            }

            if (string.IsNullOrEmpty(model.Password))
            {
                return Json(new { success = false, errors = new { Password = new[] { _localizer["PasswordRequired"].Value } } });
            }

            if (!Enum.TryParse<UserRole>(model.Role, out var userRole))
            {
                return Json(new { success = false, errors = new { Role = new[] { _localizer["InvalidRole"].Value } } });
            }

            var newUser = new UserModel
            {
                Username = model.Username.Trim(),
                Email = model.Email.Trim().ToLower(),
                FirstName = model.FirstName.Trim(),
                LastName = model.LastName.Trim(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                Role = userRole,
                IsActive = model.IsActive,
                IsEmailVerified = true, // Auto-verify admin-created accounts
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Admin created new employee: {Username}", newUser.Username);

            TempData["SuccessMessage"] = _localizer["EmployeeCreatedSuccess"].Value;
            return Json(new { success = true });
        }

        // POST: Admin/UpdateEmployee
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateEmployee(int id, EmployeeViewModel model)
        {
            if (!await _userService.IsCurrentUserAdmin())
            {
                return Json(new { success = false, errors = new { general = _localizer["Unauthorized"].Value } });
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );
                return Json(new { success = false, errors });
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return Json(new { success = false, errors = new { general = _localizer["UserNotFound"].Value } });
            }

            // Check if username exists (excluding current user)
            if (await _context.Users.AnyAsync(u => u.Username.ToLower() == model.Username.ToLower() && u.Id != id))
            {
                return Json(new { success = false, errors = new { Username = new[] { _localizer["UsernameExists"].Value } } });
            }

            // Check if email exists (excluding current user)
            if (await _context.Users.AnyAsync(u => u.Email.ToLower() == model.Email.ToLower() && u.Id != id))
            {
                return Json(new { success = false, errors = new { Email = new[] { _localizer["EmailExists"].Value } } });
            }

            if (!Enum.TryParse<UserRole>(model.Role, out var userRole))
            {
                return Json(new { success = false, errors = new { Role = new[] { _localizer["InvalidRole"].Value } } });
            }

            user.Username = model.Username.Trim();
            user.Email = model.Email.Trim().ToLower();
            user.FirstName = model.FirstName.Trim();
            user.LastName = model.LastName.Trim();
            user.Role = userRole;
            user.IsActive = model.IsActive;
            user.UpdatedAt = DateTime.UtcNow;

            // Update password if provided
            if (!string.IsNullOrEmpty(model.Password))
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Admin updated employee: {Username}", user.Username);

            TempData["SuccessMessage"] = _localizer["EmployeeUpdatedSuccess"].Value;
            return Json(new { success = true });
        }

        // POST: Admin/DeleteEmployee
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            if (!await _userService.IsCurrentUserAdmin())
            {
                TempData["ErrorMessage"] = _localizer["Unauthorized"].Value;
                return Json(new { success = false, message = _localizer["Unauthorized"].Value });
            }

            var currentUserId = HttpContext.Session.GetInt32("UserId");
            if (id == currentUserId)
            {
                return Json(new { success = false, message = _localizer["CannotDeleteYourself"].Value });
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return Json(new { success = false, message = _localizer["UserNotFound"].Value });
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Admin deleted employee: {Username}", user.Username);

            TempData["SuccessMessage"] = _localizer["EmployeeDeletedSuccess"].Value;
            return Json(new { success = true });
        }

        // POST: Admin/ToggleEmployeeStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleEmployeeStatus(int id)
        {
            if (!await _userService.IsCurrentUserAdmin())
            {
                return Json(new { success = false, message = _localizer["Unauthorized"].Value });
            }

            var currentUserId = HttpContext.Session.GetInt32("UserId");
            if (id == currentUserId)
            {
                return Json(new { success = false, message = _localizer["CannotDeactivateYourself"].Value });
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return Json(new { success = false, message = _localizer["UserNotFound"].Value });
            }

            user.IsActive = !user.IsActive;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var status = user.IsActive ? _localizer["Activated"].Value : _localizer["Deactivated"].Value;
            _logger.LogInformation("Admin {Status} employee: {Username}", status, user.Username);

            TempData["SuccessMessage"] = _localizer["EmployeeStatusUpdated", status].Value;
            return Json(new { success = true, isActive = user.IsActive });
        }
    }
}