using autocare_api.Models;

namespace autocare_api.DTOs
{
    public class WorkshopRegisterRequest
    {
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Password { get; set; } = "";

        public string WorkshopName { get; set; } = "";
        public string OwnerName { get; set; } = "";
        public AddressObject Address { get; set; } = new AddressObject();
        public WeeklyOperatingHours OperatingHours { get; set; } = new WeeklyOperatingHours();
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}
