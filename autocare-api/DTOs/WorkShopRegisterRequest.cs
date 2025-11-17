namespace autocare_api.DTOs
{
    public class WorkshopRegisterRequest
    {
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Password { get; set; } = "";

        public string WorkshopName { get; set; } = "";
        public string OwnerName { get; set; } = "";
        public string Address { get; set; } = "";
        public string OperatingHours { get; set; } = "";
    }
}
