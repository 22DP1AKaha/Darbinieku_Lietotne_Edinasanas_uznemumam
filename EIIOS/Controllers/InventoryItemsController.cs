using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public InventoryItemsController(EIIOSDbContext context, UserService userService)
        {
            _context = context;
            _userService = userService;
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

        // POST: /InventoryItems/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Manage(InventoryManagementViewModel model)
        {
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

            TempData["Success"] = "Ingredient added successfully.";
            return RedirectToAction(nameof(Manage));
        }

        // GET: /InventoryItems/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.InventoryItems.FindAsync(id);
            if (item == null) return NotFound();

            return View(item);
        }

        // POST: /InventoryItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.InventoryItems.FindAsync(id);
            if (item == null) return NotFound();

            _context.InventoryItems.Remove(item);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Ingredient removed successfully.";
            return RedirectToAction(nameof(Manage));
        }
        // POST: /InventoryItems/Update/5
        [HttpPost]
        public async Task<IActionResult> Edit(InventoryItemModel model)
        {
            if (!ModelState.IsValid)
                return RedirectToAction(nameof(Manage));

            var item = await _context.InventoryItems.FindAsync(model.Id);
            if (item == null)
                return NotFound();

            item.Name = model.Name;
            item.Unit = model.Unit;
            item.CurrentQuantity = model.CurrentQuantity;
            item.MinimumQuantity = model.MinimumQuantity;
            item.UpdatedAt = DateTime.UtcNow;

            _context.InventoryItems.Update(item);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Manage));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(InventoryManagementViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.InventoryItems = await _context.InventoryItems.OrderBy(i => i.Name).ToListAsync();
                return View("Manage", vm);
            }

            var newItem = new InventoryItemModel
            {
                Name = vm.NewItem.Name,
                Unit = vm.NewItem.Unit,
                CurrentQuantity = vm.NewItem.CurrentQuantity,
                MinimumQuantity = vm.NewItem.MinimumQuantity
            };

            _context.InventoryItems.Add(newItem);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Manage));
        }

    }
}
