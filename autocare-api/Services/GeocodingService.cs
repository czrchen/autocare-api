using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

public class GeocodingResult
{
    public double Lat { get; set; }
    public double Lng { get; set; }
}

public class GeocodingService
{
    private readonly HttpClient _http;

    public GeocodingService(HttpClient http)
    {
        _http = http;
        // Nominatim requires a User Agent
        _http.DefaultRequestHeaders.UserAgent.ParseAdd("autocare-app/1.0");
    }

    public async Task<GeocodingResult?> GeocodeAsync(string address)
    {
        if (string.IsNullOrWhiteSpace(address)) return null;

        var url =
            $"https://nominatim.openstreetmap.org/search?format=json&q={Uri.EscapeDataString(address)}";

        var json = await _http.GetStringAsync(url);

        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        if (root.GetArrayLength() == 0) return null;

        var first = root[0];

        if (!first.TryGetProperty("lat", out var latProp) ||
            !first.TryGetProperty("lon", out var lonProp))
        {
            return null;
        }

        if (!double.TryParse(latProp.GetString(), out var lat)) return null;
        if (!double.TryParse(lonProp.GetString(), out var lng)) return null;

        return new GeocodingResult { Lat = lat, Lng = lng };
    }
}
