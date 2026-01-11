using System.ComponentModel.DataAnnotations;

namespace OutdoorRentals.Web.Models;

public class RentalItem
{
    public int Id { get; set; }

    [Range(1, 100)]
    public int Quantity { get; set; }

    [Range(0, 10000)]
    public decimal PricePerDay { get; set; }

    // FK
    public int RentalId { get; set; }
    public Rental? Rental { get; set; }

    public int EquipmentId { get; set; }
    public Equipment? Equipment { get; set; }
}

