using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using EIIOS.Data;

namespace EIIOS.Controllers
{
    public class DashboardController : Controller
    {
        private readonly EIIOSDbContext _context;
        private readonly IStringLocalizer<DashboardController> _localizer;

        public DashboardController(EIIOSDbContext context, IStringLocalizer<DashboardController> localizer)
        {
            _context = context;
            _localizer = localizer;
        }

        public IActionResult Index()
        {
            // Example localized values you might want to pass to the view
            ViewBag.DashboardTitle = _localizer["Dashboard"];
            ViewBag.ChooseAction = _localizer["ChooseAction"];
            return View("Dashboard");
        }
    }
}
