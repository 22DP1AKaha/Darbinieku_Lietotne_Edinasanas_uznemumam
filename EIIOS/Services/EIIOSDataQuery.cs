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
                "category" => query.OrderBy(p => p.ProductCategories.FirstOrDefault().Category.Name)
                                   .ThenBy(p => p.Name),
                "discount" => query.OrderByDescending(p => p.DiscountProducts
                    .Where(dp => dp.Discount.IsCurrentlyActive)
                    .Max(dp => (decimal?)dp.Discount.DiscountValue) ?? 0),
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
            return await _context.Products
                .Include(p => p.ProductCategories)
                .ThenInclude(pc => pc.Category)
                .Include(p => p.DiscountProducts)
                .ThenInclude(dp => dp.Discount)
                .Where(p => p.IsActive &&
                           p.IsAvailable &&
                           p.DiscountProducts.Any(dp => dp.Discount.IsCurrentlyActive))
                .Take(count)
                .ToListAsync();
        }
    }
}