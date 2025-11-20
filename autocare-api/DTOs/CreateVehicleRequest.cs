namespace autocare_api.DTOs
{
    public class CreateVehicleRequest
    {
        public string Make { get; set; } = "";
        public string Model { get; set; } = "";
        public int Year { get; set; }
        public string Plate { get; set; } = "";
        public int Mileage { get; set; } = 0;
        public string Email { get; set; } = "";
        public string Image { get; set; } = "";

        public string Color { get; set; } = "";
        public string PurchaseDate { get; set; } = "";
    }
}
