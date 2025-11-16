using System;
using System.Collections.Generic;
using autocare_api.Models;

namespace autocare_api.Models
{
    public class ServiceRecord
    {
        public Guid Id { get; set; }

        public Guid VehicleId { get; set; }
        public Guid WorkshopId { get; set; }
        public Guid? InvoiceId { get; set; }

        public DateTime ServiceDate { get; set; }
        public int ServiceMileage { get; set; }
        public string Remarks { get; set; } = "";

        public bool IsOcrUsed { get; set; } = false;

        public Vehicle? Vehicle { get; set; }
        public User? Workshop { get; set; }
        public Invoice? Invoice { get; set; }
        public ICollection<ServiceItem>? ServiceItems { get; set; }
    }
}
