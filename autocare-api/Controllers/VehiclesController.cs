using autocare_api.Data;
using autocare_api.DTOs;
using autocare_api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using autocare_api.DTOs.Response;

namespace autocare_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehiclesController : ControllerBase
    {
        private readonly AppDbContext _db;

        public VehiclesController(AppDbContext db)
        {
            _db = db;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateVehicle(CreateVehicleRequest request)
        {
            try
            {
                // Validate required fields
                if (string.IsNullOrEmpty(request.Email))
                    return BadRequest(new { error = "Email is required" });

                if (string.IsNullOrEmpty(request.PurchaseDate))
                    return BadRequest(new { error = "Purchase date is required" });

                // Check user exists
                var user = await _db.Users.FirstOrDefaultAsync(x => x.Email == request.Email);
                if (user == null)
                    return BadRequest(new { error = "User not found" });

                // First, parse the date string
                DateTime purchaseDate;
                if (!DateTime.TryParseExact(
                    request.PurchaseDate,
                    "yyyy-MM-dd",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None,
                    out purchaseDate))
                {
                    return BadRequest(new { error = "Invalid purchase date format" });
                }

                purchaseDate = DateTime.SpecifyKind(purchaseDate, DateTimeKind.Utc);

                // Create vehicle
                var vehicle = new Vehicle
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    Manufacturer = request.Make,
                    Model = request.Model,
                    Year = request.Year,
                    PlateNumber = request.Plate,
                    CurrentMileage = request.Mileage,
                    Image = request.Image,
                    Color = request.Color,
                    PurchaseDate = purchaseDate,
                    CreatedAt = DateTime.UtcNow
                };

                _db.Vehicles.Add(vehicle);
                await _db.SaveChangesAsync();

                // Prepare response
                var response = new VehicleResponse
                {
                    Id = vehicle.Id,
                    Make = vehicle.Manufacturer,
                    Model = vehicle.Model,
                    Year = vehicle.Year,
                    PlateNumber = vehicle.PlateNumber,
                    CurrentMileage = vehicle.CurrentMileage,
                    Image = vehicle.Image,
                    Color = vehicle.Color,
                    PurchaseDate = vehicle.PurchaseDate.ToString("yyyy-MM-dd"),
                };

                return Ok(new
                {
                    success = true,
                    vehicle = response
                });
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine("DATABASE ERROR:");
                Console.WriteLine(dbEx.InnerException?.Message); // <-- add this
                Console.WriteLine(dbEx.Message);

                return StatusCode(500, new
                {
                    error = "Database error",
                    details = dbEx.InnerException?.Message ?? dbEx.Message
                });
            }
            catch (Exception ex)
            {
                // Log the full exception for debugging
                Console.WriteLine($"Error creating vehicle: {ex}");
                return StatusCode(500, new { error = "Internal server error", details = ex.Message });
            }
        }

        [HttpGet("user/{email}")]
        public async Task<IActionResult> GetUserVehicles(string email)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                return NotFound(new { error = "User not found" });

            var vehicles = await _db.Vehicles
                .Where(v => v.UserId == user.Id)
                .Select(v => new VehicleResponse
                {
                    Id = v.Id,
                    Make = v.Manufacturer,
                    Model = v.Model,
                    Year = v.Year,
                    PlateNumber = v.PlateNumber,
                    CurrentMileage = v.CurrentMileage,
                    Image = v.Image,
                    Color = v.Color,
                    PurchaseDate = v.PurchaseDate.ToString("yyyy-MM-dd"),
                })
                .ToListAsync();

            return Ok(new
            {
                count = vehicles.Count,  // ← total number
                vehicles = vehicles      // ← list of vehicles
            });
        }

        [HttpPut("{id}/mileage")]
        public async Task<IActionResult> UpdateMileage(Guid id, [FromBody] int newMileage)
        {
            var vehicle = await _db.Vehicles.FirstOrDefaultAsync(v => v.Id == id);
            if (vehicle == null)
                return NotFound(new { error = "Vehicle not found" });

            vehicle.CurrentMileage = newMileage;
            await _db.SaveChangesAsync();

            return Ok(new { success = true });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVehicle(Guid id)
        {
            var vehicle = await _db.Vehicles.FirstOrDefaultAsync(v => v.Id == id);
            if (vehicle == null)
                return NotFound(new { error = "Vehicle not found" });

            return Ok(new VehicleResponse
            {
                Id = vehicle.Id,
                Make = vehicle.Manufacturer,
                Model = vehicle.Model,
                Year = vehicle.Year,
                PlateNumber = vehicle.PlateNumber,
                CurrentMileage = vehicle.CurrentMileage,
                Image = vehicle.Image,
                Color = vehicle.Color,
                PurchaseDate = vehicle.PurchaseDate.ToString("yyyy-MM-dd")
            });
        }

    }
}