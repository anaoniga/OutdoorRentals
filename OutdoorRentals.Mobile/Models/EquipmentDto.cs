namespace OutdoorRentals.Mobile.Models
{
    public class EquipmentDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = "";
        public string? Description { get; set; }

        public decimal DailyRate { get; set; }

        public int StockTotal { get; set; }
        public int StockAvailable { get; set; }

        public int EquipmentCategoryId { get; set; }

        // doar pentru afișare în listă
        public string? EquipmentCategoryName { get; set; }
    }
}
