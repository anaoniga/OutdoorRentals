using System.ComponentModel.DataAnnotations;

namespace OutdoorRentals.Web.Models;

public class EquipmentCategory
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    public ICollection<Equipment> Equipments { get; set; } = new List<Equipment>();
}
