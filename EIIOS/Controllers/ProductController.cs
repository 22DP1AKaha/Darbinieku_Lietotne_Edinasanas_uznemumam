using EIIOS.Data;
using EIIOS.Models;
using EIIOS.Services;
using EIIOS.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace EIIOS.Controllers
{
    public class ProductController : Controller
    {
        private readonly EIIOSDbContext _context;
        private readonly IStringLocalizer<ProductController> _localizer;
        private readonly EIIOSDataQuery _dataQuery;
        private readonly UserService _userService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(
            EIIOSDbContext context,
            IStringLocalizer<ProductController> localizer,
            EIIOSDataQuery dataQuery,
            UserService userService,
            ILogger<ProductController> logger)
        {
            _context = context;
            _localizer = localizer;
            _dataQuery = dataQuery;
            _userService = userService;
            _logger = logger;
        }

        public async Task<bool> IsCurrentUserAdmin()
        {
            return await _userService.IsCurrentUserAdmin();
        }
        public async Task<bool> IsCurrentUserEmployee()
        {
            return await _userService.IsCurrentUserEmployee();
        }

        public async Task<IActionResult> Products(string searchTerm = "", string category = "", string sortBy = "name")
        {
            var viewModel = new MenuViewModel
            {
                Products = await _dataQuery.GetMenuProductsAsync(searchTerm, category, sortBy),
                Categories = await _dataQuery.GetActiveCategoriesAsync(),
                SearchTerm = searchTerm,
                Category = category,
                SortBy = sortBy
            };

            ViewBag.IsAdmin = await IsCurrentUserAdmin();

            if (ViewBag.IsAdmin)
            {
                ViewBag.AllCategories = await _context.Categories.Where(c => c.IsActive).ToListAsync();
                ViewBag.Allergens = await _context.Allergens.OrderBy(a => a.Name).ToListAsync();
                ViewBag.InventoryItems = await _context.InventoryItems.OrderBy(i => i.Name).ToListAsync();
            }

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetProduct(int id)
        {
            if (!await IsCurrentUserAdmin() && !await IsCurrentUserEmployee())
                return Unauthorized();

            var product = await _context.Products
                .Include(p => p.ProductCategories)
                .Include(p => p.ProductAllergens)
                .Include(p => p.ProductIngredients)
                    .ThenInclude(pi => pi.InventoryItem)
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
                ingredients = product.ProductIngredients.Select(pi => new
                {
                    inventoryItemId = pi.InventoryItemId,
                    quantityNeeded = pi.QuantityNeeded,
                    unit = pi.Unit
                }).ToList(),
                imageUrl = product.Image != null ? $"data:{product.Image.ContentType};base64,{product.Image.Base64Data}" : null
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct(CreateProductViewModel model)
        {
            if (!await IsCurrentUserAdmin())
            {
                return Json(new { success = false, message = _localizer["Unauthorized"].Value });
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .Select(x => $"{x.Key}: {string.Join(", ", x.Value.Errors.Select(e => e.ErrorMessage))}")
                    .ToList();

                _logger.LogWarning("Product creation validation failed. Errors: {Errors}", string.Join("; ", errors));
                return Json(new { success = false, message = _localizer["ValidationFailed"].Value });
            }

            if (model.SelectedCategoryIds == null || !model.SelectedCategoryIds.Any())
            {
                return Json(new { success = false, message = _localizer["CategoryRequired"].Value });
            }

            try
            {
                var imageService = HttpContext.RequestServices.GetRequiredService<ImageService>();

                ImageModel? image = null;
                if (model.ImageFile != null)
                {
                    image = await imageService.SaveImageAsync(model.ImageFile, model.ImageAltText);
                    if (image == null)
                    {
                        return Json(new { success = false, message = _localizer["InvalidImageFile"].Value });
                    }
                }

                var product = new ProductModel
                {
                    Name = model.Name,
                    Description = model.Description,
                    BasePrice = model.BasePrice,
                    PreparationTime = model.PreparationTime,
                    IsAvailable = model.IsAvailable,
                    IsActive = model.IsActive,
                    ImageId = image?.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    CreatedById = HttpContext.Session.GetInt32("UserId") ?? 1
                };

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                foreach (var categoryId in model.SelectedCategoryIds)
                {
                    _context.ProductCategories.Add(new ProductCategoryModel
                    {
                        ProductId = product.Id,
                        CategoryId = categoryId
                    });
                }

                if (model.SelectedAllergenIds != null)
                {
                    foreach (var allergenId in model.SelectedAllergenIds)
                    {
                        _context.ProductAllergens.Add(new ProductAllergenModel
                        {
                            ProductId = product.Id,
                            AllergenId = allergenId
                        });
                    }
                }

                if (model.Ingredients != null)
                {
                    foreach (var ingredient in model.Ingredients.Where(i => i.InventoryItemId > 0 && i.QuantityNeeded > 0))
                    {
                        _context.ProductIngredients.Add(new ProductIngredientModel
                        {
                            ProductId = product.Id,
                            InventoryItemId = ingredient.InventoryItemId,
                            QuantityNeeded = ingredient.QuantityNeeded,
                            Unit = ingredient.Unit
                        });
                    }
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Product created: {ProductName}", product.Name);
                TempData["SuccessMessage"] = _localizer["ProductCreatedSuccess"].Value;
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product");
                return Json(new { success = false, message = _localizer["ErrorCreatingProduct"].Value });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProduct(int id, CreateProductViewModel model)
        {
            if (!await IsCurrentUserAdmin())
            {
                return Json(new { success = false, message = _localizer["Unauthorized"].Value });
            }

            if (!ModelState.IsValid)
            {
                var errors = string.Join("; ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                _logger.LogWarning("Product update validation failed: {Errors}", errors);
                return Json(new { success = false, message = _localizer["ValidationFailed"].Value });
            }

            if (model.SelectedCategoryIds == null || !model.SelectedCategoryIds.Any())
            {
                return Json(new { success = false, message = _localizer["CategoryRequired"].Value });
            }

            var product = await _context.Products
                .Include(p => p.ProductCategories)
                .Include(p => p.ProductAllergens)
                .Include(p => p.ProductIngredients)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return Json(new { success = false, message = _localizer["ProductNotFound"].Value });
            }

            try
            {
                var imageService = HttpContext.RequestServices.GetRequiredService<ImageService>();

                if (model.ImageFile != null)
                {
                    if (product.ImageId.HasValue)
                    {
                        await imageService.DeleteImageAsync(product.ImageId.Value);
                    }

                    var newImage = await imageService.SaveImageAsync(model.ImageFile, model.ImageAltText);
                    if (newImage == null)
                    {
                        return Json(new { success = false, message = _localizer["InvalidImageFile"].Value });
                    }

                    product.ImageId = newImage?.Id;
                }

                product.Name = model.Name;
                product.Description = model.Description;
                product.BasePrice = model.BasePrice;
                product.PreparationTime = model.PreparationTime;
                product.IsAvailable = model.IsAvailable;
                product.IsActive = model.IsActive;
                product.UpdatedAt = DateTime.UtcNow;

                _context.ProductCategories.RemoveRange(product.ProductCategories);
                foreach (var categoryId in model.SelectedCategoryIds)
                {
                    _context.ProductCategories.Add(new ProductCategoryModel
                    {
                        ProductId = product.Id,
                        CategoryId = categoryId
                    });
                }

                _context.ProductAllergens.RemoveRange(product.ProductAllergens);
                if (model.SelectedAllergenIds != null)
                {
                    foreach (var allergenId in model.SelectedAllergenIds)
                    {
                        _context.ProductAllergens.Add(new ProductAllergenModel
                        {
                            ProductId = product.Id,
                            AllergenId = allergenId
                        });
                    }
                }

                _context.ProductIngredients.RemoveRange(product.ProductIngredients);
                if (model.Ingredients != null)
                {
                    foreach (var ingredient in model.Ingredients.Where(i => i.InventoryItemId > 0 && i.QuantityNeeded > 0))
                    {
                        _context.ProductIngredients.Add(new ProductIngredientModel
                        {
                            ProductId = product.Id,
                            InventoryItemId = ingredient.InventoryItemId,
                            QuantityNeeded = ingredient.QuantityNeeded,
                            Unit = ingredient.Unit
                        });
                    }
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Product updated: {ProductName}", product.Name);
                TempData["SuccessMessage"] = _localizer["ProductUpdatedSuccess"].Value;
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product");
                return Json(new { success = false, message = _localizer["ErrorUpdatingProduct"].Value });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            if (!await IsCurrentUserAdmin())
            {
                return Json(new { success = false, message = _localizer["Unauthorized"].Value });
            }

            // Load product with all related entities
            var product = await _context.Products
                .Include(p => p.ProductCategories)
                .Include(p => p.ProductAllergens)
                .Include(p => p.ProductIngredients)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return Json(new { success = false, message = _localizer["ProductNotFound"].Value });
            }

            try
            {
                // Check if product is used in any daily menus
                var dailyMenuItems = await _context.DailyMenuItems
                    .Where(dmi => dmi.ProductId == id)
                    .ToListAsync();

                if (dailyMenuItems.Any())
                {
                    // Remove from daily menus first
                    _context.DailyMenuItems.RemoveRange(dailyMenuItems);
                    _logger.LogInformation("Removed {Count} daily menu items for product {ProductId}", dailyMenuItems.Count, id);
                }

                // Remove related entities
                if (product.ProductCategories != null && product.ProductCategories.Any())
                {
                    _context.ProductCategories.RemoveRange(product.ProductCategories);
                }

                if (product.ProductAllergens != null && product.ProductAllergens.Any())
                {
                    _context.ProductAllergens.RemoveRange(product.ProductAllergens);
                }

                if (product.ProductIngredients != null && product.ProductIngredients.Any())
                {
                    _context.ProductIngredients.RemoveRange(product.ProductIngredients);
                }

                // Delete image if exists
                if (product.ImageId.HasValue)
                {
                    var imageService = HttpContext.RequestServices.GetRequiredService<ImageService>();
                    await imageService.DeleteImageAsync(product.ImageId.Value);
                }

                // Now delete the product
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Product deleted: {ProductName} (ID: {ProductId})", product.Name, product.Id);
                TempData["SuccessMessage"] = _localizer["ProductDeletedSuccess"].Value;
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product {ProductId}", id);
                return Json(new { success = false, message = _localizer["ErrorDeletingProduct"].Value });
            }
        }
    }
}