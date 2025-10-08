using EIIOS.Data;
using EIIOS.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace EIIOS.Controllers
{
    public class DashboardController : Controller
    {
        private readonly EIIOSDbContext _context;
        private readonly IStringLocalizer<DashboardController> _localizer;
        private readonly UserService _userService;

        public DashboardController(EIIOSDbContext context, IStringLocalizer<DashboardController> localizer, UserService userService)
        {
            _context = context;
            _localizer = localizer;
            _userService = userService;
        }

        public async Task<bool> IsCurrentUserAdmin()
        {
            return await _userService.IsCurrentUserAdmin();
        }

        public async Task<bool> IsCurrentUserEmployee()
        {
            return await _userService.IsCurrentUserEmployee();
        }

        public async Task<IActionResult> Dashboard()
        {
            // Check if user is logged in
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Check if admin
            ViewBag.IsAdmin = await IsCurrentUserAdmin();

            return View();
        }
    }
}
