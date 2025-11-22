using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace autocare_api.Models
{
    public class Service
    {
        [Key]
        public Guid Id { get; set; }

        // Foreign key → WorkshopProfile
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
    }
}
