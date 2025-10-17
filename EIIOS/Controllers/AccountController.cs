using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Antiforgery;
using EIIOS.Data;
using EIIOS.Models;
using EIIOS.ViewModels;
using EIIOS.Services;
using BCrypt.Net;
using System.Security.Cryptography;

namespace EIIOS.Controllers
{
    public class AccountController : Controller
    {
        private readonly EIIOSDbContext _context;
        private readonly IStringLocalizer<AccountController> _localizer;
        private readonly ILogger<AccountController> _logger;
        private readonly EmailService _emailService;
        private readonly IAntiforgery _antiforgery;

        public AccountController(
            EIIOSDbContext context,
            IStringLocalizer<AccountController> localizer,
            ILogger<AccountController> logger,
            EmailService emailService,
            IAntiforgery antiforgery)
        {
            _context = context;
            _localizer = localizer;
            _logger = logger;
            _emailService = emailService;
            _antiforgery = antiforgery;
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
                // First, find the user without the IsActive check
                var user = await _context.Users
                    .FirstOrDefaultAsync(u =>
                        u.Username == model.Username || u.Email == model.Username);

                if (user == null)
                {
                    ModelState.AddModelError("", _localizer["InvalidCredentials"]);
                    return View(model);
                }

                // Check password before checking account status
                bool isPasswordValid;
                if (user.PasswordHash.StartsWith("$2"))
                {
                    isPasswordValid = BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash);
                }
                else
                {
                    isPasswordValid = user.PasswordHash == model.Password;
                }

                if (!isPasswordValid)
                {
                    ModelState.AddModelError("", _localizer["InvalidCredentials"]);
                    return View(model);
                }

                // Check if account is active
                if (!user.IsActive)
                {
                    TempData["ErrorMessage"] = _localizer["AccountRestricted"].Value;
                    _logger.LogWarning("Inactive user attempted to login: {Username}", user.Username);
                    return View(model);
                }

                // Check if email is verified
                if (!user.IsEmailVerified)
                {
                    TempData["WarningMessage"] = _localizer["EmailNotVerified"].Value;
                    TempData["UnverifiedUserId"] = user.Id;
                    return RedirectToAction("VerificationRequired", new { userId = user.Id });
                }

                // Store user info in session
                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("UserName", user.FullName);
                HttpContext.Session.SetString("UserRole", user.Role.ToString());
                HttpContext.Session.SetString("Username", user.Username);

                _logger.LogInformation("User {Username} logged in successfully", user.Username);

                TempData["SuccessMessage"] = _localizer["WelcomeBack", user.FirstName].Value;

                if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                {
                    return Redirect(model.ReturnUrl);
                }

                return user.Role switch
                {
                    UserRole.Administrator => RedirectToAction("Index", "Home"),
                    UserRole.Employee => RedirectToAction("Dashboard", "Dashboard"),
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
                var existingUsername = await _context.Users
                    .AnyAsync(u => u.Username.ToLower() == model.Username.ToLower());

                if (existingUsername)
                {
                    ModelState.AddModelError("Username", _localizer["UsernameExists"]);
                    return View(model);
                }

                var existingEmail = await _context.Users
                    .AnyAsync(u => u.Email.ToLower() == model.Email.ToLower());

                if (existingEmail)
                {
                    ModelState.AddModelError("Email", _localizer["EmailExists"]);
                    return View(model);
                }

                // Generate verification token
                var verificationToken = GenerateVerificationToken();
                var tokenExpiry = DateTime.UtcNow.AddHours(24);

                var newUser = new UserModel
                {
                    Username = model.Username.Trim(),
                    Email = model.Email.Trim().ToLower(),
                    FirstName = model.FirstName.Trim(),
                    LastName = model.LastName.Trim(),
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                    Role = UserRole.Client,
                    IsEmailVerified = false,
                    EmailVerificationToken = verificationToken,
                    EmailVerificationTokenExpiry = tokenExpiry,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                // Send verification email
                var verificationLink = Url.Action(
                    "VerifyEmail",
                    "Account",
                    new { userId = newUser.Id, token = verificationToken },
                    Request.Scheme
                );

                var emailSent = await _emailService.SendVerificationEmailAsync(
                    newUser.Email,
                    newUser.FullName,
                    verificationLink!
                );

                if (!emailSent)
                {
                    _logger.LogWarning("Failed to send verification email to {Email}", newUser.Email);
                }

                _logger.LogInformation("New user registered: {Username} ({Email})", newUser.Username, newUser.Email);

                TempData["SuccessMessage"] = _localizer["RegistrationSuccess"].Value;
                TempData["InfoMessage"] = _localizer["VerificationEmailSent"].Value;

                return RedirectToAction("VerificationRequired", new { userId = newUser.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for user {Username}", model.Username);
                ModelState.AddModelError("", _localizer["RegistrationError"]);
                return View(model);
            }
        }

        // GET: Account/VerifyEmail
        [HttpGet]
        public async Task<IActionResult> VerifyEmail(int userId, string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                TempData["ErrorMessage"] = _localizer["InvalidVerificationLink"].Value;
                return RedirectToAction("Login");
            }

            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                TempData["ErrorMessage"] = _localizer["UserNotFound"].Value;
                return RedirectToAction("Login");
            }

            if (user.IsEmailVerified)
            {
                TempData["InfoMessage"] = _localizer["EmailAlreadyVerified"].Value;
                return RedirectToAction("Login");
            }

            if (user.EmailVerificationToken != token)
            {
                TempData["ErrorMessage"] = _localizer["InvalidVerificationToken"].Value;
                return RedirectToAction("Login");
            }

            if (user.EmailVerificationTokenExpiry < DateTime.UtcNow)
            {
                TempData["ErrorMessage"] = _localizer["VerificationTokenExpired"].Value;
                TempData["UnverifiedUserId"] = userId;
                return RedirectToAction("VerificationRequired", new { userId });
            }

            user.IsEmailVerified = true;
            user.EmailVerificationToken = null;
            user.EmailVerificationTokenExpiry = null;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Email verified for user {Username}", user.Username);

            TempData["SuccessMessage"] = _localizer["EmailVerifiedSuccess"].Value;
            return RedirectToAction("Login");
        }

        // GET: Account/VerificationRequired
        [HttpGet]
        public async Task<IActionResult> VerificationRequired(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null || user.IsEmailVerified)
            {
                return RedirectToAction("Login");
            }

            ViewBag.Email = user.Email;
            ViewBag.UserId = userId;
            return View();
        }

        // POST: Account/ResendVerification
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendVerification(int userId)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                TempData["ErrorMessage"] = _localizer["UserNotFound"].Value;
                return RedirectToAction("Login");
            }

            if (user.IsEmailVerified)
            {
                TempData["InfoMessage"] = _localizer["EmailAlreadyVerified"].Value;
                return RedirectToAction("Login");
            }

            // Generate new token
            var verificationToken = GenerateVerificationToken();
            var tokenExpiry = DateTime.UtcNow.AddHours(24);

            user.EmailVerificationToken = verificationToken;
            user.EmailVerificationTokenExpiry = tokenExpiry;
            await _context.SaveChangesAsync();

            // Send new verification email
            var verificationLink = Url.Action(
                "VerifyEmail",
                "Account",
                new { userId = user.Id, token = verificationToken },
                Request.Scheme
            );

            var emailSent = await _emailService.SendVerificationEmailAsync(
                user.Email,
                user.FullName,
                verificationLink!
            );

            if (emailSent)
            {
                TempData["SuccessMessage"] = _localizer["VerificationEmailResent"].Value;
            }
            else
            {
                TempData["ErrorMessage"] = _localizer["EmailSendFailed"].Value;
            }

            return RedirectToAction("VerificationRequired", new { userId });
        }

        // GET: Account/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["SuccessMessage"] = _localizer["LogoutSuccess"].Value;
            return RedirectToAction("Index", "Home");
        }

        // GET: Account/Profile
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

        // GET: Account/GetEditProfileData (for modal)
        [HttpGet]
        public async Task<IActionResult> GetEditProfileData()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return Json(new { success = false, message = _localizer["SessionExpired"].Value });
            }

            var user = await _context.Users.FindAsync(userId.Value);
            if (user == null)
            {
                return Json(new { success = false, message = _localizer["UserNotFound"].Value });
            }

            return Json(new
            {
                success = true,
                data = new
                {
                    username = user.Username,
                    firstName = user.FirstName,
                    lastName = user.LastName,
                    email = user.Email
                }
            });
        }

        // POST: Account/EditProfile (Modal)
        [HttpPost]
        public async Task<IActionResult> EditProfile([FromBody] EditProfileViewModel model)
        {
            // Validate antiforgery token
            try
            {
                await _antiforgery.ValidateRequestAsync(HttpContext);
            }
            catch
            {
                return Json(new { success = false, message = "Invalid security token. Please refresh the page and try again." });
            }

            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return Json(new { success = false, message = _localizer["SessionExpired"].Value });
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return Json(new { success = false, message = string.Join(", ", errors) });
            }

            try
            {
                var user = await _context.Users.FindAsync(userId.Value);
                if (user == null)
                {
                    return Json(new { success = false, message = _localizer["UserNotFound"].Value });
                }

                // Check if username is being changed and if it's already taken
                if (user.Username != model.Username)
                {
                    var usernameExists = await _context.Users
                        .AnyAsync(u => u.Username.ToLower() == model.Username.ToLower() && u.Id != userId.Value);

                    if (usernameExists)
                    {
                        return Json(new { success = false, message = _localizer["UsernameExists"].Value });
                    }

                    user.Username = model.Username.Trim();
                    HttpContext.Session.SetString("Username", user.Username);
                }

                user.FirstName = model.FirstName.Trim();
                user.LastName = model.LastName.Trim();
                user.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                // Update session with new full name
                HttpContext.Session.SetString("UserName", user.FullName);

                _logger.LogInformation("User {UserId} updated profile", userId.Value);

                return Json(new
                {
                    success = true,
                    message = _localizer["ProfileUpdatedSuccess"].Value,
                    data = new
                    {
                        fullName = user.FullName,
                        username = user.Username
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating profile for user {UserId}", userId.Value);
                return Json(new { success = false, message = _localizer["ProfileUpdateError"].Value });
            }
        }

        // POST: Account/ChangePassword (Modal)
        [HttpPost]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordViewModel model)
        {
            // Validate antiforgery token
            try
            {
                await _antiforgery.ValidateRequestAsync(HttpContext);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Antiforgery validation failed");
                return Json(new { success = false, message = "Invalid security token. Please refresh the page and try again." });
            }

            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return Json(new { success = false, message = _localizer["SessionExpired"].Value });
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return Json(new { success = false, message = string.Join(", ", errors) });
            }

            try
            {
                var user = await _context.Users.FindAsync(userId.Value);
                if (user == null)
                {
                    return Json(new { success = false, message = _localizer["UserNotFound"].Value });
                }

                // Verify current password
                bool isCurrentPasswordValid;
                if (user.PasswordHash.StartsWith("$2"))
                {
                    isCurrentPasswordValid = BCrypt.Net.BCrypt.Verify(model.CurrentPassword, user.PasswordHash);
                }
                else
                {
                    isCurrentPasswordValid = user.PasswordHash == model.CurrentPassword;
                }

                if (!isCurrentPasswordValid)
                {
                    return Json(new { success = false, message = _localizer["CurrentPasswordIncorrect"].Value });
                }

                // Update password
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
                user.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("User {UserId} changed password", userId.Value);

                return Json(new { success = true, message = _localizer["PasswordChangedSuccess"].Value });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password for user {UserId}", userId.Value);
                return Json(new { success = false, message = _localizer["PasswordChangeError"].Value });
            }
        }

        // Helper method to generate secure verification token
        private string GenerateVerificationToken()
        {
            var randomBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            return Convert.ToBase64String(randomBytes)
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "");
        }
    }
}