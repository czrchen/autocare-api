using System;

namespace autocare_api.Models
{
    public class Invoices
    {
        public Guid Id { get; set; }
        public string InvoiceNumber { get; set; } = "";

        public Guid UserId { get; set; }
        public Guid WorkshopId { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }

        public string PdfUrl { get; set; } = "";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User? User { get; set; }
        public User? Workshop { get; set; }
    }
}
