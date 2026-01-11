using System.ComponentModel.DataAnnotations;

namespace OutdoorRentals.Web.Models;

public class Equipment
{
    public int Id { get; set; }

    [Required, StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [Range(0, 10000)]
    public decimal DailyRate { get; set; }

    [Range(0, 1000)]
    public int StockTotal { get; set; }

    [Range(0, 1000)]
    public int StockAvailable { get; set; }

      public int EquipmentCategoryId { get; set; }
    public EquipmentCategory EquipmentCategory { get; set; } = null!;

    public ICollection<RentalItem> RentalItems { get; set; } = new List<RentalItem>();
}

