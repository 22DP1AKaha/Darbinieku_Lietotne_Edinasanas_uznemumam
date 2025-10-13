using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EIIOS.Models;

namespace EIIOS.ViewModels
{
    public class InventoryManagementViewModel
    {
        public List<InventoryItemModel> InventoryItems { get; set; } = new();

        public CreateInventoryItemViewModel NewItem { get; set; } = new();
    }

    public class CreateInventoryItemViewModel
    {
        public int? Id { get; set; } // For updates

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Unit is required.")]
        [StringLength(20)]
        public string Unit { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal CurrentQuantity { get; set; } = 0;

        [Range(0, double.MaxValue)]
        public decimal MinimumQuantity { get; set; } = 0;

        [Range(0, double.MaxValue)]
        public decimal? UnitCost { get; set; }

        // Computed property for display
        public string Status => CurrentQuantity <= MinimumQuantity ? "Low Stock" : "In Stock";
    }

}
