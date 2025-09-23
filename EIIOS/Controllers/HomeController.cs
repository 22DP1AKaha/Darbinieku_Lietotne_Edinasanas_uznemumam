using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using EIIOS.Data;

namespace EIIOS.Controllers
{
    public class HomeController : Controller
    {
        private readonly EIIOSDbContext _context;
        private readonly IStringLocalizer<HomeController> _localizer;

        public HomeController(EIIOSDbContext context, IStringLocalizer<HomeController> localizer)
        {
            _context = context;
            _localizer = localizer;
        }

        public IActionResult Index()
        {
            ViewBag.WelcomeTitle = _localizer["WelcomeTitle"];
            return View();
        }

        public IActionResult Menu()
        {
            ViewBag.Message = _localizer["MenuNotAvailable"];
            return View();
        }

        public IActionResult About()
        {
            return View();
        }
    }
}