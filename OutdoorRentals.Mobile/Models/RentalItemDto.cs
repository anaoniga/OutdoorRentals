namespace OutdoorRentals.Mobile.Models
{
    public class RentalItemDto
    {
        public int Id { get; set; }
        public int RentalId { get; set; }
        public int EquipmentId { get; set; }
        public int Quantity { get; set; }
        public decimal DailyRate { get; set; }

        public string? EquipmentName { get; set; }
    }
}
