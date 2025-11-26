using autocare_api.Models;

namespace autocare_api.Services
{
    public class DummyGeocodingService : IGeocodingService
    {
        public Task<(double Latitude, double Longitude)?> GeocodeAsync(AddressObject address)
        {
            // For now return fixed location (KL)
            return Task.FromResult<(double, double)?>((3.1390, 101.6869));
        }
    }
}
