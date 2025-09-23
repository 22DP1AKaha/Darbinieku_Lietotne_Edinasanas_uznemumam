using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using EIIOS.Data;
using EIIOS.Models;
using EIIOS.ViewModels;

namespace EIIOS.Controllers
{
    public class AccountController : Controller
    {
        private readonly EIIOSDbContext _context;
        private readonly IStringLocalizer<AccountController> _localizer;

        public AccountController(EIIOSDbContext context, IStringLocalizer<AccountController> localizer)
        {
            _context = context;
            _localizer = localizer;
        }

        // GET: Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Find user by username or email
                var user = await _context.Users
                    .FirstOrDefaultAsync(u =>
                        (u.Username == model.Username || u.Email == model.Username)
                        && u.IsActive);

                if (user == null)
                {
                    ModelState.AddModelError("", _localizer["InvalidCredentials"]);
                    return View(model);
                }

                // Verify password (use proper hashing in production!)
                if (user.PasswordHash != model.Password)
                {
                    ModelState.AddModelError("", _localizer["InvalidCredentials"]);
                    return View(model);
                }

                // TODO: Implement proper authentication
                TempData["UserId"] = user.Id;
                TempData["UserName"] = user.FullName;
                TempData["UserRole"] = user.Role.ToString();
                TempData["SuccessMessage"] = _localizer["WelcomeBack", user.FirstName];

                // Redirect based on role
                switch (user.Role)
                {
                    case UserRole.Administrator:
                        return RedirectToAction("Dashboard", "Admin");
                    case UserRole.Employee:
                        return RedirectToAction("Dashboard", "Employee");
                    case UserRole.Client:
                    default:
                        return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", _localizer["LoginError"]);
                return View(model);
            }
        }

        // GET: Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View(new UserModel());
        }

        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Check if username already exists
                var existingUsername = await _context.Users
                    .AnyAsync(u => u.Username == model.Username);

                if (existingUsername)
                {
                    ModelState.AddModelError("Username", _localizer["UsernameExists"]);
                    return View(model);
                }

                // Check if email already exists
                var existingEmail = await _context.Users
                    .AnyAsync(u => u.Email == model.Email);

                if (existingEmail)
                {
                    ModelState.AddModelError("Email", _localizer["EmailExists"]);
                    return View(model);
                }

                // Set default values
                model.Role = UserRole.Client;
                model.CreatedAt = DateTime.UtcNow;
                model.UpdatedAt = DateTime.UtcNow;
                model.IsActive = true;
                model.IsEmailVerified = false;

                // Add to database
                _context.Users.Add(model);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = _localizer["RegistrationSuccess"];
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", _localizer["RegistrationError"]);
                return View(model);
            }
        }

        // GET: Account/Logout
        public IActionResult Logout()
        {
            // Clear session data
            TempData.Clear();
            TempData["SuccessMessage"] = _localizer["LogoutSuccess"];
            return RedirectToAction("Index", "Home");
        }
    }
}
