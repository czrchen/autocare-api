namespace autocare_api.DTOs
{
    public class CreateServiceRecordRequest
    {
        public Guid VehicleId { get; set; }
        public Guid WorkshopProfileId { get; set; }
        public Guid ServiceId { get; set; }
        public Guid UserId { get; set; }

        public DateTime ServiceDate { get; set; }
        public int ServiceMileage { get; set; }
        public string Remarks { get; set; } = "";

        public string ServiceName { get; set; } = "";
        public decimal ServicePrice { get; set; }

        public string Status { get; set; } = "Scheduled";
    }
}
