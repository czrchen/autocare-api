using autocare_api.DTOs.Response;

namespace autocare_api.DTOs.Response
{
    public class ServiceGroupByWorkshop
    {
        public string WorkshopProfileId { get; set; } = string.Empty;
        public List<ServiceResponse> Services { get; set; } = new List<ServiceResponse>();
    }
}