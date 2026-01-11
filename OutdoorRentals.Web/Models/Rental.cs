using System.ComponentModel.DataAnnotations;

namespace OutdoorRentals.Web.Models;

public class Rental
{
    public int Id { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    [Required, StringLength(30)]
    public string Status { get; set; } = "Planned";
    

    // FK
    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }

    public ICollection<RentalItem> RentalItems { get; set; } = new List<RentalItem>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}

