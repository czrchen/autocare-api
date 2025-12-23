using System;
using System.Collections.Generic;
using autocare_api.Models;

namespace autocare_api.Models
{
    public class Vehicle
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        public string PlateNumber { get; set; } = "";
        public string Model { get; set; } = "";
        public string Manufacturer { get; set; } = "";
        public int Year { get; set; }
        public int CurrentMileage { get; set; }

        public DateTime CreatedAt { get; set; }
        public string Image { get; set; } = "";
        public string Color { get; set; } = "";
        public DateTime PurchaseDate { get; set; }

        public User? User { get; set; }
        public ICollection<ServiceRecord>? ServiceRecords { get; set; }
    }
}
