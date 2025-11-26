using System;
using System.Collections.Generic;
using autocare_api.Models;

namespace autocare_api.Dtos.Workshops
{
    public class WorkshopApplicationDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        public string OwnerName { get; set; } = "";
        public string WorkshopName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";

        public AddressObject Address { get; set; } = new AddressObject();
        public WeeklyOperatingHours? OperatingHours { get; set; }

        // "pending" | "approved" | "rejected"
        public string Status { get; set; } = "pending";

        public DateTime CreatedAt { get; set; }

        public string? ApprovalNotes { get; set; }
    }

    public class WorkshopApprovalRequest
    {
        public string? Notes { get; set; }
    }
}
