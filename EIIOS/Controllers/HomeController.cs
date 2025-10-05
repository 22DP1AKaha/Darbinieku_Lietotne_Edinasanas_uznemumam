using EIIOS.Data;
using EIIOS.Models;
using EIIOS.Services;
using EIIOS.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Globalization;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;
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

            // Get current logged-in user and check role
            ViewBag.IsAdmin = await IsCurrentUserAdmin();

            if (ViewBag.IsAdmin)
            {
                ViewBag.AllCategories = await _context.Categories.Where(c => c.IsActive).ToListAsync();
                ViewBag.Allergens = await _context.Allergens.OrderBy(a => a.Name).ToListAsync();
            }

            return View(viewModel);
        }

        public IActionResult About()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _context.Products
                .Include(p => p.ProductCategories)
                .Include(p => p.ProductAllergens)
                .Include(p => p.Image)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return NotFound();

            return Json(new
            {
                id = product.Id,
                name = product.Name,
                description = product.Description,
                basePrice = product.BasePrice,
                preparationTime = product.PreparationTime,
                isAvailable = product.IsAvailable,
                isActive = product.IsActive,
                categoryIds = product.ProductCategories.Select(pc => pc.CategoryId).ToList(),
                allergenIds = product.ProductAllergens.Select(pa => pa.AllergenId).ToList(),
                imageUrl = product.Image != null ? $"data:{product.Image.ContentType};base64,{product.Image.Base64Data}" : null
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct()
        {
            if (!await IsCurrentUserAdmin())
            {
                TempData["ErrorMessage"] = _localizer["Unauthorized"].Value;
                return RedirectToAction("Index");
            }

            var name = Request.Form["Name"].ToString();
            var description = Request.Form["Description"].ToString();
            var basePriceStr = Request.Form["BasePrice"].ToString();
            var prepTimeStr = Request.Form["PreparationTime"].ToString();
            var isAvailable = Request.Form["IsAvailable"].ToString() == "true";
            var isActive = Request.Form["IsActive"].ToString() == "true";
            var categoryIds = Request.Form["SelectedCategoryIds"].Select(int.Parse).ToList();
            var allergenIds = Request.Form["SelectedAllergenIds"].Select(int.Parse).ToList();
            var imageFile = Request.Form.Files.GetFile("ImageFile");

            if (!decimal.TryParse(basePriceStr, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal basePrice) || basePrice < 0)
            {
                return Json(new { success = false, message = _localizer["InvalidPriceFormat"].Value });
            }

            int? prepTime = null;
            if (!string.IsNullOrEmpty(prepTimeStr))
            {
                if (!int.TryParse(prepTimeStr, out int parsedPrepTime) || parsedPrepTime < 0)
                {
                    return Json(new { success = false, message = _localizer["InvalidPrepTimeFormat"].Value });
                }
                prepTime = parsedPrepTime;
            }

            var imageService = HttpContext.RequestServices.GetRequiredService<ImageService>();

            ImageModel? image = null;
            if (imageFile != null)
            {
                image = await imageService.SaveImageAsync(imageFile, null);
                if (image == null)
                {
                    return Json(new { success = false, message = _localizer["InvalidImageFile"].Value });
                }
            }

            var product = new ProductModel
            {
                Name = name,
                Description = description,
                BasePrice = basePrice,
                PreparationTime = prepTime,
                IsAvailable = isAvailable,
                IsActive = isActive,
                ImageId = image?.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedById = HttpContext.Session.GetInt32("UserId") ?? 1
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            foreach (var categoryId in categoryIds)
            {
                _context.ProductCategories.Add(new ProductCategoryModel
                {
                    ProductId = product.Id,
                    CategoryId = categoryId
                });
            }

            foreach (var allergenId in allergenIds)
            {
                _context.ProductAllergens.Add(new ProductAllergenModel
                {
                    ProductId = product.Id,
                    AllergenId = allergenId
                });
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = _localizer["ProductCreatedSuccess"].Value;
            return Json(new { success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProduct(int id)
        {
            if (!await IsCurrentUserAdmin())
            {
                TempData["ErrorMessage"] = _localizer["Unauthorized"].Value;
                return RedirectToAction("Index");
            }

            var name = Request.Form["Name"].ToString();
            var description = Request.Form["Description"].ToString();
            var basePriceStr = Request.Form["BasePrice"].ToString();
            var prepTimeStr = Request.Form["PreparationTime"].ToString();
            var isAvailable = Request.Form["IsAvailable"].ToString() == "true";
            var isActive = Request.Form["IsActive"].ToString() == "true";
            var categoryIds = Request.Form["SelectedCategoryIds"].Select(int.Parse).ToList();
            var allergenIds = Request.Form["SelectedAllergenIds"].Select(int.Parse).ToList();
            var imageFile = Request.Form.Files.GetFile("ImageFile");

            if (!decimal.TryParse(basePriceStr, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal basePrice) || basePrice < 0)
            {
                return Json(new { success = false, message = _localizer["InvalidPriceFormat"].Value });
            }

            int? prepTime = null;
            if (!string.IsNullOrEmpty(prepTimeStr))
            {
                if (!int.TryParse(prepTimeStr, out int parsedPrepTime) || parsedPrepTime < 0)
                {
                    return Json(new { success = false, message = _localizer["InvalidPrepTimeFormat"].Value });
                }
                prepTime = parsedPrepTime;
            }

            var product = await _context.Products
                .Include(p => p.ProductCategories)
                .Include(p => p.ProductAllergens)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return Json(new { success = false, message = _localizer["ProductNotFound"].Value });
            }

            var imageService = HttpContext.RequestServices.GetRequiredService<ImageService>();

            if (imageFile != null)
            {
                if (product.ImageId.HasValue)
                {
                    await imageService.DeleteImageAsync(product.ImageId.Value);
                }

                var newImage = await imageService.SaveImageAsync(imageFile, null);
                if (newImage == null)
                {
                    return Json(new { success = false, message = _localizer["InvalidImageFile"].Value });
                }

                product.ImageId = newImage?.Id;
            }

            product.Name = name;
            product.Description = description;
            product.BasePrice = basePrice;
            product.PreparationTime = prepTime;
            product.IsAvailable = isAvailable;
            product.IsActive = isActive;
            product.UpdatedAt = DateTime.UtcNow;

            _context.ProductCategories.RemoveRange(product.ProductCategories);
            foreach (var categoryId in categoryIds)
            {
                _context.ProductCategories.Add(new ProductCategoryModel
                {
                    ProductId = product.Id,
                    CategoryId = categoryId
                });
            }

            _context.ProductAllergens.RemoveRange(product.ProductAllergens);
            foreach (var allergenId in allergenIds)
            {
                _context.ProductAllergens.Add(new ProductAllergenModel
                {
                    ProductId = product.Id,
                    AllergenId = allergenId
                });
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = _localizer["ProductUpdatedSuccess"].Value;
            return Json(new { success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            if (!await IsCurrentUserAdmin())
            {
                TempData["ErrorMessage"] = _localizer["Unauthorized"].Value;
                return RedirectToAction("Index");
            }

            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return Json(new { success = false, message = _localizer["ProductNotFound"].Value });
            }

            var imageService = HttpContext.RequestServices.GetRequiredService<ImageService>();

            if (product.ImageId.HasValue)
            {
                await imageService.DeleteImageAsync(product.ImageId.Value);
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = _localizer["ProductDeletedSuccess"].Value;
            return Json(new { success = true });
        }

        public async Task<bool> IsCurrentUserAdmin()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null || userId == 0)
                return false;

            var user = await _context.Users.FindAsync(userId.Value);
            return user?.Role == UserRole.Administrator && user.IsActive;
        }

        public async Task<bool> IsCurrentUserEmployee()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null || userId == 0)
                return false;

            var user = await _context.Users.FindAsync(userId.Value);
            return user?.Role == UserRole.Employee && user.IsActive;
        }

        [HttpGet]
        public async Task<IActionResult> GetDiscount(int id)
        {
            if (!await IsCurrentUserAdmin())
                return Unauthorized();

            var discount = await _context.Discounts
                .Include(d => d.DiscountProducts)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (discount == null)
                return NotFound();

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
        public async Task<IActionResult> CreateDiscount()
        {
            if (!await IsCurrentUserAdmin())
            {
                TempData["ErrorMessage"] = _localizer["Unauthorized"].Value;
                return RedirectToAction("Index");
            }

            var name = Request.Form["Name"].ToString();
            var description = Request.Form["Description"].ToString();
            var discountTypeStr = Request.Form["DiscountType"].ToString();
            var discountValueStr = Request.Form["DiscountValue"].ToString();
            var startDateStr = Request.Form["StartDate"].ToString();
            var endDateStr = Request.Form["EndDate"].ToString();
            var isActive = Request.Form["IsActive"].ToString() == "true";
            var productIds = Request.Form["SelectedProductIds"].Select(int.Parse).ToList();

            if (!decimal.TryParse(discountValueStr, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal discountValue) || discountValue < 0)
            {
                return Json(new { success = false, message = _localizer["InvalidDiscountValue"].Value });
            }

            if (!DateTime.TryParse(startDateStr, out DateTime startDate))
            {
                return Json(new { success = false, message = _localizer["InvalidStartDate"].Value });
            }

            DateTime? endDate = null;
            if (!string.IsNullOrEmpty(endDateStr) && DateTime.TryParse(endDateStr, out DateTime parsedEndDate))
            {
                endDate = parsedEndDate;
            }

            if (!Enum.TryParse<DiscountType>(discountTypeStr, out var discountType))
            {
                return Json(new { success = false, message = _localizer["InvalidDiscountType"].Value });
            }

            var discount = new DiscountModel
            {
                Name = name,
                Description = description,
                DiscountType = discountType,
                DiscountValue = discountValue,
                StartDate = startDate,
                EndDate = endDate,
                IsActive = isActive,
                CreatedAt = DateTime.UtcNow,
                CreatedById = HttpContext.Session.GetInt32("UserId") ?? 1
            };

            _context.Discounts.Add(discount);
            await _context.SaveChangesAsync();

            foreach (var productId in productIds)
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
        public async Task<IActionResult> UpdateDiscount(int id)
        {
            if (!await IsCurrentUserAdmin())
            {
                TempData["ErrorMessage"] = _localizer["Unauthorized"].Value;
                return RedirectToAction("Index");
            }

            var name = Request.Form["Name"].ToString();
            var description = Request.Form["Description"].ToString();
            var discountTypeStr = Request.Form["DiscountType"].ToString();
            var discountValueStr = Request.Form["DiscountValue"].ToString();
            var startDateStr = Request.Form["StartDate"].ToString();
            var endDateStr = Request.Form["EndDate"].ToString();
            var isActive = Request.Form["IsActive"].ToString() == "true";
            var productIds = Request.Form["SelectedProductIds"].Select(int.Parse).ToList();

            if (!decimal.TryParse(discountValueStr, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal discountValue) || discountValue < 0)
            {
                return Json(new { success = false, message = _localizer["InvalidDiscountValue"].Value });
            }

            if (!DateTime.TryParse(startDateStr, out DateTime startDate))
            {
                return Json(new { success = false, message = _localizer["InvalidStartDate"].Value });
            }

            DateTime? endDate = null;
            if (!string.IsNullOrEmpty(endDateStr) && DateTime.TryParse(endDateStr, out DateTime parsedEndDate))
            {
                endDate = parsedEndDate;
            }

            if (!Enum.TryParse<DiscountType>(discountTypeStr, out var discountType))
            {
                return Json(new { success = false, message = _localizer["InvalidDiscountType"].Value });
            }

            var discount = await _context.Discounts
                .Include(d => d.DiscountProducts)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (discount == null)
            {
                return Json(new { success = false, message = _localizer["DiscountNotFound"].Value });
            }

            discount.Name = name;
            discount.Description = description;
            discount.DiscountType = discountType;
            discount.DiscountValue = discountValue;
            discount.StartDate = startDate;
            discount.EndDate = endDate;
            discount.IsActive = isActive;

            _context.DiscountProducts.RemoveRange(discount.DiscountProducts);
            foreach (var productId in productIds)
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
            if (!await IsCurrentUserAdmin())
                return Unauthorized();

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
    }
}