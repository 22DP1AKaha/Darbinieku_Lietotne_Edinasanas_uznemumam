using Microsoft.EntityFrameworkCore;
using EIIOS.Data;
using EIIOS.Models;

namespace EIIOS.Services
{
    public class EIIOSDataQuery
    {
        private readonly EIIOSDbContext _context;

        public EIIOSDataQuery(EIIOSDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets filtered and sorted products with related data
        /// </summary>
        public async Task<List<ProductModel>> GetMenuProductsAsync(
            string searchTerm = "",
            string category = "",
            string sortBy = "name")
        {
            var now = DateTime.UtcNow;

            var query = _context.Products
                .Include(p => p.ProductCategories)
                .ThenInclude(pc => pc.Category)
                .Include(p => p.DiscountProducts)
                .ThenInclude(dp => dp.Discount)
                .Include(p => p.Image)
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
                "category" => query.OrderBy(p => p.ProductCategories
                                                   .OrderBy(pc => pc.Category.Name)
                                                   .Select(pc => pc.Category.Name)
                                                   .FirstOrDefault())
                                   .ThenBy(p => p.Name),
                "discount" => query
                    .Select(p => new
                    {
                        Product = p,
                        MaxDiscount = p.DiscountProducts
                                       .Where(dp => dp.Discount.StartDate <= now && dp.Discount.EndDate >= now)
                                       .Max(dp => (decimal?)dp.Discount.DiscountValue) ?? 0
                    })
                    .OrderByDescending(x => x.MaxDiscount)
                    .Select(x => x.Product),
                _ => query.OrderBy(p => p.Name)
            };

            return await query.ToListAsync();
        }

        /// <summary>
        /// Gets all active categories ordered by display order
        /// </summary>
        public async Task<List<CategoryModel>> GetActiveCategoriesAsync()
        {
            return await _context.Categories
                .Where(c => c.IsActive)
                .OrderBy(c => c.DisplayOrder)
                .ToListAsync();
        }

        /// <summary>
        /// Gets a single product by ID with all related data
        /// </summary>
        public async Task<ProductModel?> GetProductByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.ProductCategories)
                .ThenInclude(pc => pc.Category)
                .Include(p => p.DiscountProducts)
                .ThenInclude(dp => dp.Discount)
                .Include(p => p.ProductAllergens)
                .ThenInclude(pa => pa.Allergen)
                .Include(p => p.ProductIngredients)
                .ThenInclude(pi => pi.InventoryItem)
                .FirstOrDefaultAsync(p => p.Id == id && p.IsActive && p.IsAvailable);
        }

        /// <summary>
        /// Gets products with active discounts
        /// </summary>
        public async Task<List<ProductModel>> GetDiscountedProductsAsync(int count = 6)
        {
            var now = DateTime.UtcNow;

            return await _context.Products
                .Include(p => p.ProductCategories)
                .ThenInclude(pc => pc.Category)
                .Include(p => p.DiscountProducts)
                .ThenInclude(dp => dp.Discount)
                .Where(p => p.IsActive &&
                           p.IsAvailable &&
                           p.DiscountProducts.Any(dp => dp.Discount.StartDate <= now && dp.Discount.EndDate >= now))
                .Take(count)
                .ToListAsync();
        }
    }
}
