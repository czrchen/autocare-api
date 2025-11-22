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
    public class ServiceRecordController : ControllerBase
    {
        private readonly AppDbContext _db;

        public ServiceRecordController(AppDbContext db)
        {
            _db = db;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateServiceRecord(CreateServiceRecordRequest request)
        {
            var vehicle = await _db.Vehicles.FirstOrDefaultAsync(v => v.Id == request.VehicleId);
            if (vehicle == null)
                return BadRequest(new { error = "Vehicle not found" });

            var workshop = await _db.WorkshopProfiles.FirstOrDefaultAsync(w => w.Id == request.WorkshopProfileId);
            if (workshop == null)
                return BadRequest(new { error = "Workshop not found" });

            var record = new ServiceRecord
            {
                Id = Guid.NewGuid(),
                VehicleId = request.VehicleId,
                WorkshopProfileId = request.WorkshopProfileId,
                ServiceId = request.ServiceId,
                UserId = request.UserId,
                ServiceDate = DateTime.SpecifyKind(request.ServiceDate, DateTimeKind.Utc),
                ServiceMileage = request.ServiceMileage,
                Remarks = request.Remarks,
                Status = request.Status,
                ServiceItems = new List<ServiceItem>()
            };

            _db.ServiceRecords.Add(record);
            await _db.SaveChangesAsync();

            return Ok(new { success = true, id = record.Id });
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllRecords()
        {
            var records = await _db.ServiceRecords
                .Include(r => r.Vehicle)
                .Include(r => r.Service)
                .Include(r => r.WorkshopProfile)
                .Include(r => r.Vehicle.User)
                .Select(r => new ServiceRecordResponse
                {
                    Id = r.Id,
                    VehicleId = r.VehicleId,
                    WorkshopId = r.WorkshopProfileId,
                    ServiceId = r.ServiceId,

                    UserId = r.Vehicle.UserId,
                    UserName = r.Vehicle.User.FullName,
                    UserEmail = r.Vehicle.User.Email,
                    UserPhone = r.Vehicle.User.Phone,

                    VehicleName = r.Vehicle.Manufacturer + " " + r.Vehicle.Model,
                    VehiclePlate = r.Vehicle.PlateNumber,
                    WorkshopName = r.WorkshopProfile.WorkshopName,
                    ServiceName = r.Service.Name,

                    ServiceDate = r.ServiceDate,
                    ServiceMileage = r.ServiceMileage,
                    Remarks = r.Remarks,
                    Status = r.Status,
                   InvoiceId = r.InvoiceId
                })
                .ToListAsync();

            var vehicles = await _db.Vehicles
                .Select(v => new { v.Id, v.UserId })
                .ToListAsync();

            var vehicleToUser = vehicles.ToDictionary(x => x.Id, x => x.UserId);

            var byUser = records
                .GroupBy(r => vehicleToUser[r.VehicleId])
                .ToDictionary(g => g.Key.ToString(), g => g.ToList());

            var byWorkshop = records
                .GroupBy(r => r.WorkshopId)
                .ToDictionary(g => g.Key.ToString(), g => g.ToList());

            var byService = records
                .GroupBy(r => r.ServiceId)
                .ToDictionary(g => g.Key.ToString(), g => g.ToList());

            return Ok(new
            {
                byUser,
                byWorkshop,
                byService
            });
        }

        // PUT: api/ServiceRecord/{id}/status
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] string status)
        {
            var record = await _db.ServiceRecords.FindAsync(id);
            if (record == null)
            {
                return NotFound();
            }

            record.Status = status;
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}
