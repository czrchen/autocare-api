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
                // Check user exists (by Email)
                var user = await _db.Users.FirstOrDefaultAsync(x => x.Email == request.Email);
                if (user == null)
                    return BadRequest(new { error = "User not found" });

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
                };

                _db.Vehicles.Add(vehicle);
                await _db.SaveChangesAsync();

                // Prepare clean DTO response
                var response = new VehicleResponse
                {
                    Id = vehicle.Id,
                    Make = vehicle.Manufacturer,
                    Model = vehicle.Model,
                    Year = vehicle.Year,
                    PlateNumber = vehicle.PlateNumber,
                    CurrentMileage = vehicle.CurrentMileage,
                    Image = vehicle.Image
                };

                return Ok(new
                {
                    success = true,
                    vehicle = response
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
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
                    Image = v.Image
                })
                .ToListAsync();

            return Ok(new
            {
                count = vehicles.Count,  // ← total number
                vehicles = vehicles      // ← list of vehicles
            });
        }
    }
}
