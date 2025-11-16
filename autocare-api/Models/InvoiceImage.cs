using System;

namespace autocare_api.Models
{
    public class InvoiceImage
    {
        public Guid Id { get; set; }
        public Guid ServiceRecordId { get; set; }

        public string ImageUrl { get; set; } = "";
        public string ExtractedJson { get; set; } = "";

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        public ServiceRecord? ServiceRecord { get; set; }
    }
}
