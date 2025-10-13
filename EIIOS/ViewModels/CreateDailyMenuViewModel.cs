using System.ComponentModel.DataAnnotations;

namespace EIIOS.ViewModels
{
    public class CreateDailyMenuViewModel
    {
        [Required]
        [DataType(DataType.Date)]
        public DateTime MenuDate { get; set; } = DateTime.Today;

        [Required]
        public List<int> ProductIds { get; set; } = new List<int>();

        public List<decimal?> SpecialPrices { get; set; } = new List<decimal?>();

        public bool IsActive { get; set; } = true;
    }
}