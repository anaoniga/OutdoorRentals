using System.ComponentModel.DataAnnotations;

namespace OutdoorRentals.Web.Models;

public class Payment
{
    public int Id { get; set; }

    [Range(0, 100000)]
    public decimal Amount { get; set; }

    public DateTime PaidAt { get; set; } = DateTime.Now;

    [Required, StringLength(30)]
    public string Method { get; set; } = "Cash";
    // Cash / Card

    [Required, StringLength(30)]
    public string Status { get; set; } = "Paid";
    // Paid / Pending

    // FK
    public int RentalId { get; set; }
    public Rental? Rental { get; set; }
}

