using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
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

        public AccountController(
            EIIOSDbContext context,
            IStringLocalizer<AccountController> localizer,
            ILogger<AccountController> logger,
            EmailService emailService)
        {
            _context = context;
            _localizer = localizer;
            _logger = logger;
            _emailService = emailService;
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
                var user = await _context.Users
                    .FirstOrDefaultAsync(u =>
                        (u.Username == model.Username || u.Email == model.Username)
                        && u.IsActive);

                if (user == null)
                {
                    ModelState.AddModelError("", _localizer["InvalidCredentials"]);
                    return View(model);
                }

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