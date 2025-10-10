using EIIOS.Data;
using EIIOS.Models;
using EIIOS.Services;
using EIIOS.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Linq;
using System.Threading.Tasks;

namespace EIIOS.Controllers
{
    public class ProductionController : Controller
    {
        private readonly EIIOSDbContext _context;
        private readonly IViewLocalizer _localizer;
        private readonly InventoryConsumptionService _inventoryService;

        public ProductionController(EIIOSDbContext context, IViewLocalizer localizer, InventoryConsumptionService inventoryService)
        {
            _context = context;
            _localizer = localizer;
            _inventoryService = inventoryService;
        }

        // GET: /Production/SelectProduct
        public async Task<IActionResult> SelectProduct()
        {
            var products = await _context.Products
                                         .OrderBy(p => p.Name)
                                         .ToListAsync();
            return View(products);
        }

        // GET: /Production/Create/{id}
        [HttpGet]
        public async Task<IActionResult> Create(int id)
        {
            var product = await _context.Products
                .Include(p => p.ProductIngredients)
                .ThenInclude(pi => pi.InventoryItem)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return NotFound(); // ✅ return value here

            var vm = new ProduceProductViewModel
            {
                ProductId = product.Id,
                ProductName = product.Name,
                Ingredients = product.ProductIngredients.Select(pi => new ProduceProductViewModel.IngredientDisplay
                {
                    Name = pi.InventoryItem?.Name ?? "Unknown",
                    QuantityNeeded = pi.QuantityNeeded,
                    Unit = pi.Unit
                }).ToList()


            };

            return View(vm); // ✅ return value at the end
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProduceProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _inventoryService.ConsumeIngredientsAsync(model.ProductId, model.QuantityProduced);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View("Create", model);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("SelectProduct");
        }

    }
}