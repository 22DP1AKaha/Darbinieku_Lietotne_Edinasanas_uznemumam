using EIIOS.Data;
using EIIOS.Models;
using EIIOS.Services;
using EIIOS.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace EIIOS.Controllers
{
    public class HomeController : Controller
    {
        private readonly EIIOSDbContext _context;
        private readonly IStringLocalizer<HomeController> _localizer;
        private readonly EIIOSDataQuery _dataQuery;
        private readonly UserService _userService;

        public HomeController(EIIOSDbContext context, IStringLocalizer<HomeController> localizer, EIIOSDataQuery dataQuery, UserService userService)
        {
            _context = context;
            _localizer = localizer;
            _dataQuery = dataQuery;
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

        public async Task<IActionResult> Index(string searchTerm = "", string category = "", string sortBy = "name")
        {
            var today = DateTime.Today;

            var todayMenu = await _context.DailyMenus
                .Include(dm => dm.DailyMenuItems)
                    .ThenInclude(dmi => dmi.Product)
                        .ThenInclude(p => p.Image)
                .Include(dm => dm.DailyMenuItems)
                    .ThenInclude(dmi => dmi.Product)
                        .ThenInclude(p => p.ProductCategories)
                            .ThenInclude(pc => pc.Category)
                .Include(dm => dm.DailyMenuItems)
                    .ThenInclude(dmi => dmi.Product)
                        .ThenInclude(p => p.ProductAllergens)
                            .ThenInclude(pa => pa.Allergen)
                .Include(dm => dm.DailyMenuItems)
                    .ThenInclude(dmi => dmi.Product)
                        .ThenInclude(p => p.DiscountProducts)
                            .ThenInclude(dp => dp.Discount)
                .FirstOrDefaultAsync(dm => dm.MenuDate.Date == today && dm.IsActive);

            var viewModel = new MenuViewModel
            {
                Products = await _dataQuery.GetMenuProductsAsync(searchTerm, category, sortBy),
                Categories = await _dataQuery.GetActiveCategoriesAsync(),
                SearchTerm = searchTerm,
                Category = category,
                SortBy = sortBy,
                TodayMenu = todayMenu,
                TodayMenuItems = todayMenu?.DailyMenuItems?
                    .Where(dmi => dmi.IsAvailable)
                    .OrderBy(dmi => dmi.DisplayOrder)
                    .ToList() ?? new List<DailyMenuItemModel>()
            };

            ViewBag.IsAdmin = await IsCurrentUserAdmin();
            ViewBag.IsEmployee = await IsCurrentUserEmployee();
            ViewBag.IsLoggedIn = HttpContext.Session.GetInt32("UserId").HasValue;

            // Always load products for today's menu (for all users to see)
            if (todayMenu != null)
            {
                ViewBag.TodayMenuProducts = todayMenu.DailyMenuItems
                    .Where(dmi => dmi.IsAvailable)
                    .Select(dmi => dmi.Product)
                    .ToList();
            }

            // Load all products only for admin/employee (for management purposes)
            if (ViewBag.IsAdmin || ViewBag.IsEmployee)
            {
                ViewBag.AllCategories = await _context.Categories.Where(c => c.IsActive).ToListAsync();
                ViewBag.Allergens = await _context.Allergens.OrderBy(a => a.Name).ToListAsync();
                ViewBag.AllProducts = await _context.Products
                    .Where(p => p.IsActive && p.IsAvailable)
                    .OrderBy(p => p.Name)
                    .ToListAsync();
            }

            return View(viewModel);
        }

        public IActionResult About()
        {
            return View();
        }

        #region Daily Menu Management

        [HttpGet]
        public async Task<IActionResult> GetDailyMenu(int id)
        {
            if (!await IsCurrentUserAdmin() && !await IsCurrentUserEmployee())
                return Unauthorized();

            var menu = await _context.DailyMenus
                .Include(dm => dm.DailyMenuItems)
                    .ThenInclude(dmi => dmi.Product)
                .FirstOrDefaultAsync(dm => dm.Id == id);

            if (menu == null) return NotFound();

            return Json(new
            {
                id = menu.Id,
                menuDate = menu.MenuDate.ToString("yyyy-MM-dd"),
                isActive = menu.IsActive,
                items = menu.DailyMenuItems.OrderBy(dmi => dmi.DisplayOrder).Select(dmi => new
                {
                    id = dmi.Id,
                    productId = dmi.ProductId,
                    productName = dmi.Product.Name,
                    displayOrder = dmi.DisplayOrder,
                    specialPrice = dmi.SpecialPrice,
                    isAvailable = dmi.IsAvailable
                }).ToList()
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetDailyMenuByDate(string date)
        {
            if (!await IsCurrentUserAdmin() && !await IsCurrentUserEmployee())
                return Unauthorized();

            if (!DateTime.TryParse(date, out var menuDate))
                return BadRequest(new { message = "Invalid date format" });

            var menu = await _context.DailyMenus
                .Include(dm => dm.DailyMenuItems)
                    .ThenInclude(dmi => dmi.Product)
                .FirstOrDefaultAsync(dm => dm.MenuDate.Date == menuDate.Date);

            if (menu == null)
                return Json(new { exists = false });

            return Json(new
            {
                exists = true,
                id = menu.Id,
                menuDate = menu.MenuDate.ToString("yyyy-MM-dd"),
                isActive = menu.IsActive,
                items = menu.DailyMenuItems.OrderBy(dmi => dmi.DisplayOrder).Select(dmi => new
                {
                    id = dmi.Id,
                    productId = dmi.ProductId,
                    productName = dmi.Product.Name,
                    displayOrder = dmi.DisplayOrder,
                    specialPrice = dmi.SpecialPrice,
                    isAvailable = dmi.IsAvailable
                }).ToList()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDailyMenu(CreateDailyMenuViewModel model)
        {
            if (!await IsCurrentUserAdmin() && !await IsCurrentUserEmployee())
            {
                return Json(new { success = false, message = _localizer["Unauthorized"].Value });
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

            if (model.ProductIds == null || !model.ProductIds.Any())
            {
                return Json(new { success = false, message = _localizer["SelectAtLeastOneProduct"].Value });
            }

            // Check if menu already exists for this date
            var existingMenu = await _context.DailyMenus
                .FirstOrDefaultAsync(dm => dm.MenuDate.Date == model.MenuDate.Date);

            if (existingMenu != null)
            {
                return Json(new { success = false, message = _localizer["MenuExistsForDate"].Value });
            }

            var dailyMenu = new DailyMenuModel
            {
                MenuDate = model.MenuDate.Date,
                IsActive = model.IsActive,
                CreatedAt = DateTime.UtcNow,
                CreatedById = HttpContext.Session.GetInt32("UserId")
            };

            _context.DailyMenus.Add(dailyMenu);
            await _context.SaveChangesAsync();

            // Add menu items
            for (int i = 0; i < model.ProductIds.Count; i++)
            {
                var menuItem = new DailyMenuItemModel
                {
                    DailyMenuId = dailyMenu.Id,
                    ProductId = model.ProductIds[i],
                    DisplayOrder = i + 1,
                    SpecialPrice = (model.SpecialPrices != null && i < model.SpecialPrices.Count)
                        ? model.SpecialPrices[i]
                        : null,
                    IsAvailable = true
                };

                _context.DailyMenuItems.Add(menuItem);
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = _localizer["DailyMenuCreatedSuccess"].Value;
            return Json(new { success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateDailyMenu(int id, CreateDailyMenuViewModel model)
        {
            if (!await IsCurrentUserAdmin() && !await IsCurrentUserEmployee())
            {
                return Json(new { success = false, message = _localizer["Unauthorized"].Value });
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

            if (model.ProductIds == null || !model.ProductIds.Any())
            {
                return Json(new { success = false, message = _localizer["SelectAtLeastOneProduct"].Value });
            }

            var menu = await _context.DailyMenus
                .Include(dm => dm.DailyMenuItems)
                .FirstOrDefaultAsync(dm => dm.Id == id);

            if (menu == null)
            {
                return Json(new { success = false, message = _localizer["MenuNotFound"].Value });
            }

            menu.MenuDate = model.MenuDate.Date;
            menu.IsActive = model.IsActive;

            // Remove old items
            _context.DailyMenuItems.RemoveRange(menu.DailyMenuItems);

            // Add new items
            for (int i = 0; i < model.ProductIds.Count; i++)
            {
                var menuItem = new DailyMenuItemModel
                {
                    DailyMenuId = menu.Id,
                    ProductId = model.ProductIds[i],
                    DisplayOrder = i + 1,
                    SpecialPrice = (model.SpecialPrices != null && i < model.SpecialPrices.Count)
                        ? model.SpecialPrices[i]
                        : null,
                    IsAvailable = true
                };

                _context.DailyMenuItems.Add(menuItem);
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = _localizer["DailyMenuUpdatedSuccess"].Value;
            return Json(new { success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDailyMenu(int id)
        {
            if (!await IsCurrentUserAdmin() && !await IsCurrentUserEmployee())
            {
                TempData["ErrorMessage"] = _localizer["Unauthorized"].Value;
                return RedirectToAction("Index");
            }

            var menu = await _context.DailyMenus.FindAsync(id);
            if (menu == null)
            {
                return Json(new { success = false, message = _localizer["MenuNotFound"].Value });
            }

            _context.DailyMenus.Remove(menu);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = _localizer["DailyMenuDeletedSuccess"].Value;
            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDailyMenus()
        {
            if (!await IsCurrentUserAdmin() && !await IsCurrentUserEmployee())
                return Unauthorized();

            var menus = await _context.DailyMenus
                .Include(dm => dm.DailyMenuItems)
                    .ThenInclude(dmi => dmi.Product)
                .OrderByDescending(dm => dm.MenuDate)
                .Take(30) // Limit to last 30 menus
                .ToListAsync();

            return Json(menus.Select(dm => new
            {
                id = dm.Id,
                menuDate = dm.MenuDate.ToString("yyyy-MM-dd"),
                isActive = dm.IsActive,
                itemCount = dm.DailyMenuItems.Count,
                items = dm.DailyMenuItems.OrderBy(dmi => dmi.DisplayOrder).Select(dmi => new
                {
                    productId = dmi.ProductId,
                    productName = dmi.Product.Name,
                    specialPrice = dmi.SpecialPrice
                }).ToList()
            }));
        }

        #endregion

        #region Discount Management

        [HttpGet]
        public async Task<IActionResult> GetDiscount(int id)
        {
            if (!await IsCurrentUserAdmin()) return Unauthorized();

            var discount = await _context.Discounts
                .Include(d => d.DiscountProducts)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (discount == null) return NotFound();

            return Json(new
            {
                id = discount.Id,
                name = discount.Name,
                description = discount.Description,
                discountType = discount.DiscountType.ToString(),
                discountValue = discount.DiscountValue,
                startDate = discount.StartDate.ToString("yyyy-MM-dd"),
                endDate = discount.EndDate?.ToString("yyyy-MM-dd"),
                isActive = discount.IsActive,
                productIds = discount.DiscountProducts.Select(dp => dp.ProductId).ToList()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDiscount(CreateDiscountViewModel model)
        {
            if (!await IsCurrentUserAdmin())
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

            if (!Enum.TryParse<DiscountType>(model.DiscountType, out var discountType))
            {
                return Json(new { success = false, errors = new { DiscountType = new[] { _localizer["InvalidDiscountType"].Value } } });
            }

            var discount = new DiscountModel
            {
                Name = model.Name,
                Description = model.Description,
                DiscountType = discountType,
                DiscountValue = model.DiscountValue,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                IsActive = model.IsActive,
                CreatedAt = DateTime.UtcNow,
                CreatedById = HttpContext.Session.GetInt32("UserId") ?? 1
            };

            _context.Discounts.Add(discount);
            await _context.SaveChangesAsync();

            foreach (var productId in model.SelectedProductIds)
            {
                _context.DiscountProducts.Add(new DiscountProductModel
                {
                    DiscountId = discount.Id,
                    ProductId = productId
                });
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = _localizer["DiscountCreatedSuccess"].Value;
            return Json(new { success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateDiscount(int id, CreateDiscountViewModel model)
        {
            if (!await IsCurrentUserAdmin())
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

            if (!Enum.TryParse<DiscountType>(model.DiscountType, out var discountType))
            {
                return Json(new { success = false, errors = new { DiscountType = new[] { _localizer["InvalidDiscountType"].Value } } });
            }

            var discount = await _context.Discounts
                .Include(d => d.DiscountProducts)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (discount == null)
            {
                return Json(new { success = false, errors = new { general = _localizer["DiscountNotFound"].Value } });
            }

            discount.Name = model.Name;
            discount.Description = model.Description;
            discount.DiscountType = discountType;
            discount.DiscountValue = model.DiscountValue;
            discount.StartDate = model.StartDate;
            discount.EndDate = model.EndDate;
            discount.IsActive = model.IsActive;

            _context.DiscountProducts.RemoveRange(discount.DiscountProducts);
            foreach (var productId in model.SelectedProductIds)
            {
                _context.DiscountProducts.Add(new DiscountProductModel
                {
                    DiscountId = discount.Id,
                    ProductId = productId
                });
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = _localizer["DiscountUpdatedSuccess"].Value;
            return Json(new { success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDiscount(int id)
        {
            if (!await IsCurrentUserAdmin())
            {
                TempData["ErrorMessage"] = _localizer["Unauthorized"].Value;
                return RedirectToAction("Index");
            }

            var discount = await _context.Discounts.FindAsync(id);
            if (discount == null)
            {
                return Json(new { success = false, message = _localizer["DiscountNotFound"].Value });
            }

            _context.Discounts.Remove(discount);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = _localizer["DiscountDeletedSuccess"].Value;
            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDiscounts()
        {
            if (!await IsCurrentUserAdmin()) return Unauthorized();

            var discounts = await _context.Discounts
                .Include(d => d.DiscountProducts)
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync();

            return Json(discounts.Select(d => new
            {
                id = d.Id,
                name = d.Name,
                description = d.Description,
                discountType = d.DiscountType.ToString(),
                discountValue = d.DiscountValue,
                startDate = d.StartDate.ToString("yyyy-MM-dd"),
                endDate = d.EndDate?.ToString("yyyy-MM-dd"),
                isActive = d.IsActive,
                productIds = d.DiscountProducts.Select(dp => dp.ProductId).ToList()
            }));
        }

        #endregion
    }
}