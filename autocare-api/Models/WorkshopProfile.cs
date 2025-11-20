using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace autocare_api.Models
{
    public class WorkshopProfile
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        public string WorkshopName { get; set; } = "";
        public AddressObject Address { get; set; } = new AddressObject();
        public WeeklyOperatingHours OperatingHours { get; set; } = new WeeklyOperatingHours();
        public double Rating { get; set; }

        public User? User { get; set; }

        public ICollection<Service>? Services { get; set; }

    }
}
