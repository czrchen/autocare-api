using autocare_api.Models;
using autocare_api.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/debug/geocode")]
public class GeocodeDebugController : ControllerBase
{
    private readonly IGeocodingService _geo;

    public GeocodeDebugController(IGeocodingService geo)
    {
        _geo = geo;
    }

    [HttpGet]
    public async Task<IActionResult> Test(
        [FromQuery] string street,
        [FromQuery] string postcode,
        [FromQuery] string city)
    {
        var address = new AddressObject
        {
            Street = street,
            Postcode = postcode,
            City = city,
            State = "Wilayah Persekutuan Kuala Lumpur",
            Country = "Malaysia"
        };

        var result = await _geo.GeocodeAsync(address);

        if (result == null)
            return BadRequest("Could not geocode address");

        return Ok(new
        {
            latitude = result.Value.Latitude,
            longitude = result.Value.Longitude
        });
    }
}
