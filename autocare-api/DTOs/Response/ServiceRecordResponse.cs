namespace autocare_api.DTOs.Response
{
    public class ServiceRecordResponse
    {
        public Guid Id { get; set; }
        public Guid VehicleId { get; set; }
        public Guid WorkshopId { get; set; }
        public Guid ServiceId { get; set; }

        public Guid UserId { get; set; }
        public string UserName { get; set; } = "";
        public string UserEmail { get; set; } = "";
        public string UserPhone { get; set; } = "";
        public string VehicleName { get; set; } = "";
        public string VehiclePlate { get; set; }
        public string WorkshopName { get; set; } = "";
        public string ServiceName { get; set; } = "";

        public DateTime ServiceDate { get; set; }
        public int ServiceMileage { get; set; }

        public string? Remarks { get; set; }
        public string? Status { get; set; }

        public Guid? InvoiceId { get; set; }
    }
}
