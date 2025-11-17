namespace autocare_api.DTOs
{
    public class DriverRegisterRequest
    {
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Password { get; set; } = "";
    }
}
