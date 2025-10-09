using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EIIOS.Data;
using EIIOS.Services;
using EIIOS.ViewModels;

namespace EIIOS.Controllers
{
    public class ProductionController : Controller
    {
        private readonly EIIOSDbContext _context;
        private readonly InventoryConsumptionService _inventoryService;
        private readonly UserService _userService;

        public ProductionController(
            EIIOSDbContext context,
            InventoryConsumptionService inventoryService,
            UserService userService)
        {
            _context = context;
            _inventoryService = inventoryService;
            _userService = userService;
        }

        // GET: Production/Create/5
        public async Task<IActionResult> Create(int id)
        {
            var product = await _context.Products
                .Include(p => p.ProductIngredients)
                .ThenInclude(pi => pi.InventoryItem)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return NotFound();

            var vm = new ProduceProductViewModel
            {
                ProductId = product.Id,
                ProductName = product.Name,
                Ingredients = product.ProductIngredients?.Select(pi => new ProduceProductViewModel.IngredientDisplay
                {
                    Name = pi.InventoryItem.Name,
                    QuantityNeeded = pi.QuantityNeeded,
                    Unit = pi.Unit
                }).ToList()
            };

            return View(vm);
        }

        // POST: Production/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProduceProductViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (!await _userService.IsCurrentUserEmployee() && !await _userService.IsCurrentUserAdmin())
                return Forbid();

            try
            {
                await _inventoryService.ConsumeIngredientsAsync(model.ProductId, model.QuantityProduced);
                TempData["Success"] = $"Produced {model.QuantityProduced} {model.ProductName}. Inventory updated successfully.";
                return RedirectToAction("Index", "InventoryItems");
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Create", new { id = model.ProductId });
            }
        }
    }
}
