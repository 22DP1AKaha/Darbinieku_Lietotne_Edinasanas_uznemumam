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
            // Manually parse form data
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
                    return Json(new { success = false, message = _localizer["InvalidImageFile"].Value });
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
                CreatedById = 1
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

            return Json(new { success = true, message = _localizer["ProductCreatedSuccess"].Value });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProduct(int id)
        {
            // Manually parse the form data to avoid culture issues
            var name = Request.Form["Name"].ToString();
            var description = Request.Form["Description"].ToString();
            var basePriceStr = Request.Form["BasePrice"].ToString();
            var prepTimeStr = Request.Form["PreparationTime"].ToString();
            var isAvailable = Request.Form["IsAvailable"].ToString() == "true";
            var isActive = Request.Form["IsActive"].ToString() == "true";
            var categoryIds = Request.Form["SelectedCategoryIds"].Select(int.Parse).ToList();
            var allergenIds = Request.Form["SelectedAllergenIds"].Select(int.Parse).ToList();
            var imageFile = Request.Form.Files.GetFile("ImageFile");

            // Parse decimal using invariant culture
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
                return Json(new { success = false, message = _localizer["ProductNotFound"].Value });

            var imageService = HttpContext.RequestServices.GetRequiredService<ImageService>();

            if (imageFile != null)
            {
                if (product.ImageId.HasValue)
                {
                    await imageService.DeleteImageAsync(product.ImageId.Value);
                }

                var newImage = await imageService.SaveImageAsync(imageFile, null);
                if (newImage == null)
                    return Json(new { success = false, message = _localizer["InvalidImageFile"].Value });

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

            return Json(new { success = true, message = _localizer["ProductUpdatedSuccess"].Value });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
                return Json(new { success = false, message = _localizer["ProductNotFound"].Value });

            var imageService = HttpContext.RequestServices.GetRequiredService<ImageService>();

            if (product.ImageId.HasValue)
            {
                await imageService.DeleteImageAsync(product.ImageId.Value);
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = _localizer["ProductDeletedSuccess"].Value });
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
    }
}