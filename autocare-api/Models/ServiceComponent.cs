using System;

namespace autocare_api.Models
{
    public class ServiceComponent
    {
        public Guid Id { get; set; }

        public Guid ServiceId { get; set; }
        public Service Service { get; set; } = null!;

        public ComponentType ComponentType { get; set; }
    }
}
