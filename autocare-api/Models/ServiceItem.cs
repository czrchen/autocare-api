using System;

namespace autocare_api.Models
{
    public class ServiceItem
    {
        public Guid Id { get; set; }
        public Guid ServiceRecordId { get; set; }

        public string ItemName { get; set; } = "";
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }

        public ServiceRecord? ServiceRecord { get; set; }
    }
}
