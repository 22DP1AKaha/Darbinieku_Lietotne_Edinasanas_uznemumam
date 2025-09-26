using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EIIOS.Models;
using EIIOS.Data;

namespace EIIOS.Controllers
{
    public class MenuController : Controller
    {
        private readonly EIIOSDbContext _context;

        public MenuController(EIIOSDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchTerm = "", string category = "", string sortBy = "name")
        {
            var query = _context.Products
                .Include(p => p.ProductCategories)
                .ThenInclude(pc => pc.Category)
                .Include(p => p.DiscountProducts)
                .ThenInclude(dp => dp.Discount)
                .Where(p => p.IsActive && p.IsAvailable);

            // Apply search filter
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(p => p.Name.Contains(searchTerm) ||
                                        (p.Description != null && p.Description.Contains(searchTerm)));
            }

            // Apply category filter
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(p => p.ProductCategories.Any(pc => pc.Category.Name == category));
            }

            // Apply sorting
            query = sortBy.ToLower() switch
            {
                "price" => query.OrderBy(p => p.BasePrice),
                "pricedesc" => query.OrderByDescending(p => p.BasePrice),
                "category" => query.OrderBy(p => p.ProductCategories.FirstOrDefault().Category.Name).ThenBy(p => p.Name),
                "discount" => query.OrderByDescending(p => p.DiscountProducts
                    .Where(dp => dp.Discount.IsCurrentlyActive)
                    .Max(dp => (decimal?)dp.Discount.DiscountValue) ?? 0),
                _ => query.OrderBy(p => p.Name)
            };

            var products = await query.ToListAsync();
            var categories = await _context.Categories.Where(c => c.IsActive).OrderBy(c => c.DisplayOrder).ToListAsync();

            ViewBag.SearchTerm = searchTerm;
            ViewBag.Category = category;
            ViewBag.SortBy = sortBy;
            ViewBag.Categories = categories;

            return View(products);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _context.Products
                .Include(p => p.ProductCategories)
                .ThenInclude(pc => pc.Category)
                .Include(p => p.DiscountProducts)
                .ThenInclude(dp => dp.Discount)
                .Include(p => p.ProductAllergens)
                .ThenInclude(pa => pa.Allergen)
                .Include(p => p.ProductIngredients)
                .FirstOrDefaultAsync(p => p.Id == id && p.IsActive && p.IsAvailable);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            // This would typically integrate with your cart system
            // For now, we'll just return a JSON response
            var product = await _context.Products.FindAsync(productId);

            if (product == null || !product.IsActive || !product.IsAvailable)
            {
                return Json(new { success = false, message = "Product not found or unavailable" });
            }

            // Here you would add logic to add the item to the user's cart
            // This could be session-based, database-based, or cookie-based depending on your implementation

            return Json(new { success = true, message = $"Added {product.Name} to cart" });
        }
    }
}