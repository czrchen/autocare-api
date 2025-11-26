using System.Threading.Tasks;
using autocare_api.Models; // for AddressObject

namespace autocare_api.Services
{
    public interface IGeocodingService
    {
        Task<(double Latitude, double Longitude)?> GeocodeAsync(AddressObject address);
    }
}
