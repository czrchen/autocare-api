using System;
using System.Collections.Generic;

namespace autocare_api.Models
{
    public enum WorkshopApprovalStatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2
    }

    public class WorkshopProfile
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        public string WorkshopName { get; set; } = "";
        public AddressObject Address { get; set; } = new AddressObject();
        public WeeklyOperatingHours OperatingHours { get; set; } = new WeeklyOperatingHours();
        public double Rating { get; set; }
        public string? ApprovalNotes { get; set; }

        public WorkshopApprovalStatus ApprovalStatus { get; set; } = WorkshopApprovalStatus.Pending;

        public DateTime? ReviewedAt { get; set; }
        public Guid? ReviewedByAdminId { get; set; }

        public bool EmailNotificationsRequested { get; set; } = false;
        public bool EmailNotificationsConfirmed { get; set; } = false;

        public User? User { get; set; }

        public ICollection<Service> Services { get; set; } = new List<Service>();

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}
