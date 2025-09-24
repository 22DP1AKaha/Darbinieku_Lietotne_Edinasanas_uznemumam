using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using EIIOS.Data;
using EIIOS.Models;
using EIIOS.ViewModels;
using BCrypt.Net;


namespace EIIOS.Controllers
{
    public class AccountController : Controller
    {
        private readonly EIIOSDbContext _context;
        private readonly IStringLocalizer<AccountController> _localizer;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            EIIOSDbContext context,
            IStringLocalizer<AccountController> localizer,
            ILogger<AccountController> logger)
        {
            _context = context;
            _localizer = localizer;
            _logger = logger;
        }

        // GET: Account/Login
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginViewModel { ReturnUrl = returnUrl });
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

                // Verify password - Use BCrypt for production!
                bool isPasswordValid;
                if (user.PasswordHash.StartsWith("$2"))
                {
                    // BCrypt hash
                    isPasswordValid = BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash);
                }
                else
                {
                    // Plain text (for development/testing only)
                    isPasswordValid = user.PasswordHash == model.Password;
                }

                if (!isPasswordValid)
                {
                    ModelState.AddModelError("", _localizer["InvalidCredentials"]);
                    return View(model);
                }

                // Check if email is verified (if required)
                if (!user.IsEmailVerified)
                {
                    TempData["WarningMessage"] = _localizer["EmailNotVerified"];
                }

                // Store user info in session (simple approach)
                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("UserName", user.FullName);
                HttpContext.Session.SetString("UserRole", user.Role.ToString());
                HttpContext.Session.SetString("Username", user.Username);

                // Log successful login
                _logger.LogInformation("User {Username} logged in successfully", user.Username);

                TempData["SuccessMessage"] = _localizer["WelcomeBack", user.FirstName].Value;

                // Redirect based on role or return URL
                if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                {
                    return Redirect(model.ReturnUrl);
                }

                return user.Role switch
                {
                    UserRole.Administrator => RedirectToAction("Index", "Home"),
                    UserRole.Employee => RedirectToAction("Dashboard", "Employee"),
                    UserRole.Client => RedirectToAction("Index", "Home"),
                    _ => RedirectToAction("Index", "Home")
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for user {Username}", model.Username);
                ModelState.AddModelError("", _localizer["LoginError"]);
                return View(model);
            }
        }

        // GET: Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Check if username already exists
                var existingUsername = await _context.Users
                    .AnyAsync(u => u.Username.ToLower() == model.Username.ToLower());

                if (existingUsername)
                {
                    ModelState.AddModelError("Username", _localizer["UsernameExists"]);
                    return View(model);
                }

                // Check if email already exists
                var existingEmail = await _context.Users
                    .AnyAsync(u => u.Email.ToLower() == model.Email.ToLower());

                if (existingEmail)
                {
                    ModelState.AddModelError("Email", _localizer["EmailExists"]);
                    return View(model);
                }

                // Create new user
                var newUser = new UserModel
                {
                    Username = model.Username.Trim(),
                    Email = model.Email.Trim().ToLower(),
                    FirstName = model.FirstName.Trim(),
                    LastName = model.LastName.Trim(),
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password), // Hash password
                    Role = UserRole.Client,
                    IsEmailVerified = false, // Set to true for now, implement email verification later
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                _logger.LogInformation("New user registered: {Username} ({Email})", newUser.Username, newUser.Email);

                TempData["SuccessMessage"] = _localizer["RegistrationSuccess"];
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for user {Username}", model.Username);
                ModelState.AddModelError("", _localizer["RegistrationError"]);
                return View(model);
            }
        }

        // GET: Account/Logout
        public IActionResult Logout()
        {
            // Clear session
            HttpContext.Session.Clear();

            TempData["SuccessMessage"] = _localizer["LogoutSuccess"];
            return RedirectToAction("Index", "Home");
        }

        // GET: Account/Profile (for logged in users)
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login");
            }

            var user = await _context.Users.FindAsync(userId.Value);
            if (user == null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login");
            }

            return View(user);
        }

        // Helper method to check if user is logged in
        private bool IsUserLoggedIn()
        {
            return HttpContext.Session.GetInt32("UserId") != null;
        }

        // Helper method to get current user
        private async Task<UserModel?> GetCurrentUserAsync()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return null;

            return await _context.Users.FindAsync(userId.Value);
        }
    }
}
