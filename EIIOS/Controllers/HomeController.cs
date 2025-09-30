using EIIOS.Data;
using EIIOS.Models;
using EIIOS.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using EIIOS.Services;
namespace EIIOS.Controllers
{
    public class HomeController : Controller
    {
        private readonly EIIOSDbContext _context;
        private readonly IStringLocalizer<HomeController> _localizer;
        private readonly EIIOSDataQuery _dataQuery;

        public HomeController(EIIOSDbContext context, IStringLocalizer<HomeController> localizer, EIIOSDataQuery dataQuery)
        {
            _context = context;
            _localizer = localizer;
            _dataQuery = dataQuery;
        }

        public async Task<IActionResult> Index(string searchTerm = "", string category = "", string sortBy = "name")
        {
            var viewModel = new MenuViewModel
            {
                Products = await _dataQuery.GetMenuProductsAsync(searchTerm, category, sortBy),
                Categories = await _dataQuery.GetActiveCategoriesAsync(),
                SearchTerm = searchTerm,
                Category = category,
                SortBy = sortBy
            };

            ViewBag.WelcomeTitle = _localizer["WelcomeTitle"];

            return View(viewModel);
        }

        public IActionResult About()
        {
            return View();
        }
    }
}