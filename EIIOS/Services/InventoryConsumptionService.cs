using EIIOS.Data;
using EIIOS.Models;
using Microsoft.EntityFrameworkCore;

namespace EIIOS.Services
{
    public class InventoryConsumptionService
    {
        private readonly EIIOSDbContext _context;

        public InventoryConsumptionService(EIIOSDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ConsumeIngredientsAsync(int productId, decimal quantityProduced)
        {
            var ingredients = await _context.ProductIngredients
                .Include(pi => pi.InventoryItem)
                .Where(pi => pi.ProductId == productId)
                .ToListAsync();

            if (!ingredients.Any())
                return false;

            foreach (var ingredient in ingredients)
            {
                var item = ingredient.InventoryItem;
                if (item == null)
                    continue;

                var totalUsed = ingredient.QuantityNeeded * quantityProduced;

                if (item.CurrentQuantity < totalUsed)
                {
                    throw new InvalidOperationException(
                        $"Not enough '{item.Name}' in inventory. Required: {totalUsed} {item.Unit}, Available: {item.CurrentQuantity} {item.Unit}");
                }

                item.CurrentQuantity -= totalUsed;
                item.UpdatedAt = DateTime.UtcNow;

            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
