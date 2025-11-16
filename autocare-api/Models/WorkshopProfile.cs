using System;

namespace autocare_api.Models
{
    public class WorkshopProfile
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        public string WorkshopName { get; set; } = "";
        public string Address { get; set; } = "";
        public string OperatingHours { get; set; } = "";
        public double Rating { get; set; }

        public User? User { get; set; }
    }
}
