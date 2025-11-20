using autocare_api.Data;
using autocare_api.DTOs;
using autocare_api.Models;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using autocare_api.DTOs.Response;
using System.Text;

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
                ServiceDate = DateTime.SpecifyKind(request.ServiceDate, DateTimeKind.Utc),
                ServiceMileage = request.ServiceMileage,
                Remarks = request.Remarks,
                Status = request.Status,
                ServiceItems = new List<ServiceItem>()  // empty for now
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
                    WorkshopName = r.WorkshopProfile.WorkshopName,
                    ServiceName = r.Service.Name,

                    ServiceDate = r.ServiceDate,
                    ServiceMileage = r.ServiceMileage,
                    Remarks = r.Remarks,
                    Status = r.Status
                })
                .ToListAsync();

            // Load Vehicle → User mapping
            var vehicles = await _db.Vehicles
                .Select(v => new { v.Id, v.UserId })
                .ToListAsync();

            var vehicleToUser = vehicles.ToDictionary(x => x.Id, x => x.UserId);

            // GROUP BY USER
            var byUser = records
                .GroupBy(r => vehicleToUser[r.VehicleId])
                .ToDictionary(g => g.Key.ToString(), g => g.ToList());

            // GROUP BY WORKSHOP
            var byWorkshop = records
                .GroupBy(r => r.WorkshopId)
                .ToDictionary(g => g.Key.ToString(), g => g.ToList());

            // GROUP BY SERVICE TYPE
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
    }


}
