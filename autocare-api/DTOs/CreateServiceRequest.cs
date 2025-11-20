namespace autocare_api.DTOs
{
    public class CreateServiceRequest
    {
        public Guid WorkshopProfileId { get; set; }
        public string Name { get; set; } = "";

        public string Category { get; set; } = "";
        public string Description { get; set; } = "";
        public int DurationMinutes { get; set; }
        public decimal Price { get; set; }
    }
}
