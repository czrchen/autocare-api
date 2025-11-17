namespace autocare_api.DTOs.Response
{
    public class VehicleResponse
    {
        public Guid Id { get; set; }
        public string Make { get; set; } = "";
        public string Model { get; set; } = "";
        public int Year { get; set; }
        public string PlateNumber { get; set; } = "";
        public int CurrentMileage { get; set; } = 0;

        public string Image { get; set; } = "";
    }
}
