using System;

namespace autocare_api.Models
{
    public class ServiceRecord
    {
        public Guid Id { get; set; }

        public Guid VehicleId { get; set; }

        // CHANGE: Link to WorkshopProfile
        public Guid WorkshopProfileId { get; set; }
        public WorkshopProfile? WorkshopProfile { get; set; }

        public DateTime ServiceDate { get; set; }
        public int ServiceMileage { get; set; }
        public string Remarks { get; set; } = "";
        public Guid UserId { get; set; }
        public User User { get; set; }
        // NEW: Link to service
        public Guid ServiceId { get; set; }
        public Service? Service { get; set; }
        public string Status { get; set; } = "";
        public Guid? InvoiceId { get; set; }

        public bool ReminderSent { get; set; } = false;

        // Navigation
        public Vehicle? Vehicle { get; set; }
        public Invoices? Invoice { get; set; }

        // Optional, keep as empty list for now
        public ICollection<ServiceItem>? ServiceItems { get; set; }
    }
}
