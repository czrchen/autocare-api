using autocare_api.Models;
using autocare_api.Services;
using System.Net.Http;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

public class GeocodingService : IGeocodingService
{
    private readonly HttpClient _http;
    private readonly ILogger<GeocodingService> _logger;
    private readonly string _apiKey;

    public GeocodingService(
        HttpClient http,
        ILogger<GeocodingService> logger,
        IConfiguration config)
    {
        _http = http;
        _logger = logger;

        _apiKey = config["OpenCage:ApiKey"]
                  ?? throw new InvalidOperationException("Missing OpenCage:ApiKey config");

        _http.DefaultRequestHeaders.UserAgent.ParseAdd(
            "autocare-app/1.0 (dleong45@gmail.com)"
        );
    }

    public async Task<(double Latitude, double Longitude)?> GeocodeAsync(AddressObject address)
    {
        if (address == null)
        {
            _logger.LogWarning("GeocodeAsync called with null address");
            return null;
        }

        var fullAddress =
            $"{address.Street}, {address.Postcode} {address.City}, {address.State}, {address.Country}"
            .Trim()
            .Trim(',');

        _logger.LogInformation("Geocoding address (OpenCage): {Address}", fullAddress);

        if (string.IsNullOrWhiteSpace(fullAddress))
        {
            _logger.LogWarning("Empty address, skipping geocoding");
            return null;
        }

        var url =
            $"https://api.opencagedata.com/geocode/v1/json" +
            $"?q={Uri.EscapeDataString(fullAddress)}" +
            $"&key={_apiKey}" +
            $"&limit=1" +
            $"&countrycode=my";

        try
        {
            var response = await _http.GetAsync(url);
            var json = await response.Content.ReadAsStringAsync();

            _logger.LogInformation("OpenCage HTTP {StatusCode}, body: {Json}",
                response.StatusCode, json);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("OpenCage returned non-success status {StatusCode}", response.StatusCode);
                return null;
            }

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (!root.TryGetProperty("results", out var results) ||
                results.ValueKind != JsonValueKind.Array ||
                results.GetArrayLength() == 0)
            {
                _logger.LogWarning("No results from OpenCage for {Address}", fullAddress);
                return null;
            }

            var first = results[0];
            var geometry = first.GetProperty("geometry");

            var lat = geometry.GetProperty("lat").GetDouble();
            var lng = geometry.GetProperty("lng").GetDouble();

            _logger.LogInformation("Geocoded {Address} => {Lat}, {Lng}",
                fullAddress, lat, lng);

            return (lat, lng);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling OpenCage for {Address}", fullAddress);
            return null;
        }
    }
}
