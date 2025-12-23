using System;
using System.Collections.Generic;
using autocare_api.Models;

namespace autocare_api.Models
{
    public class User
    {
        public Guid Id { get; set; }

        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string PasswordHash { get; set; } = "";

        public string Phone { get; set; } = "";
        public string Role { get; set; } = "";
        public bool EmailNotificationsRequested { get; set; } = false;
        public bool EmailNotificationsConfirmed { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Vehicle>? Vehicles { get; set; }
        public ICollection<ServiceRecord>? ServiceRecords { get; set; }
    }
}
