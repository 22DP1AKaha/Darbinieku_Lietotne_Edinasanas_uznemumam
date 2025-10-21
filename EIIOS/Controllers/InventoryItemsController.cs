using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using EIIOS.Data;
using EIIOS.Models;
using EIIOS.Services;
using EIIOS.ViewModels;

namespace EIIOS.Controllers
{
    public class InventoryItemsController : Controller
    {
        private readonly EIIOSDbContext _context;
        private readonly UserService _userService;
        private readonly IStringLocalizer<InventoryItemsController> _localizer;

        public InventoryItemsController(EIIOSDbContext context, UserService userService, IStringLocalizer<InventoryItemsController> localizer)
        {
            _context = context;
            _userService = userService;
            _localizer = localizer;
        }

        // GET: /InventoryItems/Manage
        public async Task<IActionResult> Manage()
        {
            var vm = new InventoryManagementViewModel
            {
                InventoryItems = await _context.InventoryItems.OrderBy(i => i.Name).ToListAsync()
            };

            return View(vm);
        }

        // POST: /InventoryItems/Manage (Add new ingredient)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Manage(InventoryManagementViewModel model)
        {
            if (model.NewItem.CurrentQuantity < 0)
                ModelState.AddModelError("NewItem.CurrentQuantity", _localizer["QuantityCannotBeNegative"]);
            if (model.NewItem.MinimumQuantity < 0)
                ModelState.AddModelError("NewItem.MinimumQuantity", _localizer["MinimumQuantityCannotBeNegative"]);
            if (model.NewItem.UnitCost < 0)
                ModelState.AddModelError("NewItem.UnitCost", _localizer["UnitCostCannotBeNegative"]);

            if (!ModelState.IsValid)
            {
                model.InventoryItems = await _context.InventoryItems.OrderBy(i => i.Name).ToListAsync();
                return View(model);
            }

            var newItem = new InventoryItemModel
            {
                Name = model.NewItem.Name,
                Unit = model.NewItem.Unit,
                CurrentQuantity = model.NewItem.CurrentQuantity,
                MinimumQuantity = model.NewItem.MinimumQuantity,
                UnitCost = model.NewItem.UnitCost
            };

            _context.InventoryItems.Add(newItem);
            await _context.SaveChangesAsync();

            TempData["Success"] = _localizer["IngredientAdded"].Value;
            return RedirectToAction(nameof(Manage));
        }

        // GET: /InventoryItems/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.InventoryItems.FindAsync(id);
            if (item == null)
                return NotFound();

            return View(item);
        }

        // POST: /InventoryItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.InventoryItems.FindAsync(id);
            if (item == null)
                return NotFound();

            _context.InventoryItems.Remove(item);
            await _context.SaveChangesAsync();

            TempData["Success"] = _localizer["IngredientRemoved"].Value;
            return RedirectToAction(nameof(Manage));
        }

        // POST: /InventoryItems/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(InventoryItemModel model)
        {
            if (model.CurrentQuantity < 0)
                ModelState.AddModelError(nameof(model.CurrentQuantity), _localizer["QuantityCannotBeNegative"]);
            if (model.MinimumQuantity < 0)
                ModelState.AddModelError(nameof(model.MinimumQuantity), _localizer["MinimumQuantityCannotBeNegative"]);
            if (model.UnitCost < 0)
                ModelState.AddModelError(nameof(model.UnitCost), _localizer["UnitCostCannotBeNegative"]);

            if (!ModelState.IsValid)
                return RedirectToAction(nameof(Manage));

            var item = await _context.InventoryItems.FindAsync(model.Id);
            if (item == null)
                return NotFound();

            item.Name = model.Name;
            item.Unit = model.Unit;
            item.CurrentQuantity = model.CurrentQuantity;
            item.MinimumQuantity = model.MinimumQuantity;
            item.UnitCost = model.UnitCost;
            item.UpdatedAt = DateTime.UtcNow;

            _context.InventoryItems.Update(item);
            await _context.SaveChangesAsync();

            TempData["Success"] = _localizer["IngredientUpdated"].Value;
            return RedirectToAction(nameof(Manage));
        }
    }
}
