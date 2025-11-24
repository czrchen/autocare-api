using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace autocare_api.Models
{
    public class Service
    {
        [Key]
        public Guid Id { get; set; }

        // Foreign key to WorkshopProfile
        [Required]
        public Guid WorkshopProfileId { get; set; }

        [ForeignKey("WorkshopProfileId")]
        public WorkshopProfile? WorkshopProfile { get; set; }

        // Service basic info
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = "";

        public string Status { get; set; } = "Active";

        [Required]
        public string Category { get; set; } = "";

        [Required]
        public string Description { get; set; } = "";

        // Duration in minutes
        [Required]
        public int DurationMinutes { get; set; }

        // Price
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        // Timestamps
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // NEW  Component links, a service can affect multiple car components
        public ICollection<ServiceComponent> Components { get; set; } = new List<ServiceComponent>();

        // Optional navigation if you want to go Service -> ServiceRecords
        public ICollection<ServiceRecord> ServiceRecords { get; set; } = new List<ServiceRecord>();
    }
}
