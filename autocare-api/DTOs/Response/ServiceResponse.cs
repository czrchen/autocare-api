namespace autocare_api.DTOs.Response
{
    public class ServiceResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";

        public string Category { get; set; } = "";
        public string Description { get; set; } = "";
        public decimal Price { get; set; }
        public int DurationMinutes { get; set; }

        public Guid WorkshopProfileId { get; set; }
        public string Status { get; set; }
    }
}
