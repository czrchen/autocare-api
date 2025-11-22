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
    public class ServicesController : ControllerBase
    {
        private readonly AppDbContext _db;
        public ServicesController(AppDbContext db)
        {
            _db = db;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateService([FromBody] CreateServiceRequest request)
        {
            if (request == null)
                return BadRequest(new { error = "Invalid request" });

            var workshop = await _db.WorkshopProfiles
                .FirstOrDefaultAsync(w => w.Id == request.WorkshopProfileId);

            if (workshop == null)
                return NotFound(new { error = "Workshop not found" });

            var service = new Service
            {
                Id = Guid.NewGuid(),
                WorkshopProfileId = request.WorkshopProfileId,
                Name = request.Name,
                Category = request.Category,
                Description = request.Description,
                DurationMinutes = request.DurationMinutes,
                Price = request.Price,
                Status = "Active",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _db.Services.Add(service);
            await _db.SaveChangesAsync();

            var response = new ServiceResponse
            {
                Id = service.Id,
                Name = service.Name,
                Category = service.Category,
                Description = service.Description,
                Price = service.Price,
                DurationMinutes = service.DurationMinutes
            };

            return Ok(new
            {
                success = true,
                service = response
            });
        }

        [HttpGet("all-workshops")]
        public async Task<IActionResult> GetAllWorkshopServices()
        {
            // Fetch all services from the database
            var allServices = await _db.Services.ToListAsync();

            // Group the services by WorkshopProfileId
            var groupedServices = allServices
                .GroupBy(s => s.WorkshopProfileId)
                .Select(group => new ServiceGroupByWorkshop
                {
                    // Convert Guid to string for the key
                    WorkshopProfileId = group.Key.ToString(),
                    Services = group.Select(s => new ServiceResponse
                    {
                        Id = s.Id,
                        WorkshopProfileId = s.WorkshopProfileId,
                        Name = s.Name,
                        Category = s.Category,
                        Description = s.Description,
                        DurationMinutes = s.DurationMinutes,
                        Price = s.Price
                    }).ToList()
                })
                .ToList();

            return Ok(groupedServices);
        }

        [HttpGet("workshop/{email}")]
        public async Task<IActionResult> GetWorkshopServices(string email)
        {
            // 1) Find user
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                return NotFound(new { error = "User not found" });

            // 2) Find workshop belonging to user
            var workshop = await _db.WorkshopProfiles.FirstOrDefaultAsync(w => w.UserId == user.Id);
            if (workshop == null)
                return NotFound(new { error = "Workshop not found" });

            // 3) Get services
            var services = await _db.Services
                .Where(s => s.WorkshopProfileId == workshop.Id)
                .Select(s => new
                {
                    Id = s.Id,
                    Name = s.Name,
                    Category = s.Category,
                    Description = s.Description,
                    Price = s.Price,
                    DurationMinutes = s.DurationMinutes,
                    Status = s.Status
                })
                .ToListAsync();

            return Ok(new
            {
                count = services.Count,
                services
            });
        }

        [HttpPut("{id}/activate")]
        public async Task<IActionResult> ActivateService(Guid id)
        {
            var service = await _db.Services.FirstOrDefaultAsync(s => s.Id == id);
            if (service == null)
                return NotFound(new { error = "Service not found" });

            service.Status = "Active";
            service.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return Ok(new { success = true });
        }

        [HttpPut("{id}/deactivate")]
        public async Task<IActionResult> DeactivateService(Guid id)
        {
            var service = await _db.Services.FirstOrDefaultAsync(s => s.Id == id);
            if (service == null)
                return NotFound(new { error = "Service not found" });

            service.Status = "Inactive";
            service.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return Ok(new { success = true });
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateService(Guid id, [FromBody] UpdateServiceRequest request)
        {
            var service = await _db.Services.FirstOrDefaultAsync(s => s.Id == id);
            if (service == null)
                return NotFound(new { error = "Service not found" });

            service.Name = request.Name;
            service.Category = request.Category;
            service.Description = request.Description;
            service.Price = request.Price;
            service.DurationMinutes = request.DurationMinutes;
            service.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                service = new ServiceResponse
                {
                    Id = service.Id,
                    WorkshopProfileId = service.WorkshopProfileId,
                    Name = service.Name,
                    Category = service.Category,
                    Description = service.Description,
                    Price = service.Price,
                    DurationMinutes = service.DurationMinutes
                }
            });
        }


    }
}
