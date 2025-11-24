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

        // POST: /api/services/create
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

            // Build ServiceComponents from ComponentTypes
            var components = new List<ServiceComponent>();

            foreach (var typeString in request.ComponentTypes.Distinct())
            {
                if (Enum.TryParse<ComponentType>(typeString, ignoreCase: true, out var parsed))
                {
                    components.Add(new ServiceComponent
                    {
                        Id = Guid.NewGuid(),
                        ServiceId = service.Id,
                        Service = service,
                        ComponentType = parsed
                    });
                }
            }

            service.Components = components;

            _db.Services.Add(service);
            await _db.SaveChangesAsync();

            var response = new ServiceResponse
            {
                Id = service.Id,
                WorkshopProfileId = service.WorkshopProfileId,
                Name = service.Name,
                Category = service.Category,
                Description = service.Description,
                Price = service.Price,
                DurationMinutes = service.DurationMinutes,
                ComponentTypes = service.Components
                    .Select(c => c.ComponentType.ToString())
                    .ToList()
            };

            return Ok(new
            {
                success = true,
                service = response
            });
        }

        // GET: /api/services/all-workshops
        [HttpGet("all-workshops")]
        public async Task<IActionResult> GetAllWorkshopServices()
        {
            var allServices = await _db.Services
                .Include(s => s.Components)
                .ToListAsync();

            var groupedServices = allServices
                .GroupBy(s => s.WorkshopProfileId)
                .Select(group => new ServiceGroupByWorkshop
                {
                    WorkshopProfileId = group.Key.ToString(),
                    Services = group.Select(s => new ServiceResponse
                    {
                        Id = s.Id,
                        WorkshopProfileId = s.WorkshopProfileId,
                        Name = s.Name,
                        Category = s.Category,
                        Description = s.Description,
                        DurationMinutes = s.DurationMinutes,
                        Price = s.Price,
                        ComponentTypes = s.Components
                            .Select(c => c.ComponentType.ToString())
                            .ToList()
                    }).ToList()
                })
                .ToList();

            return Ok(groupedServices);
        }

        // GET: /api/services/workshop/{email}
        [HttpGet("workshop/{email}")]
        public async Task<IActionResult> GetWorkshopServices(string email)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                return NotFound(new { error = "User not found" });

            var workshop = await _db.WorkshopProfiles.FirstOrDefaultAsync(w => w.UserId == user.Id);
            if (workshop == null)
                return NotFound(new { error = "Workshop not found" });

            var services = await _db.Services
                .Where(s => s.WorkshopProfileId == workshop.Id)
                .Include(s => s.Components)
                .Select(s => new
                {
                    Id = s.Id,
                    Name = s.Name,
                    Category = s.Category,
                    Description = s.Description,
                    Price = s.Price,
                    DurationMinutes = s.DurationMinutes,
                    Status = s.Status,
                    componentTypes = s.Components
                        .Select(c => c.ComponentType.ToString())
                        .ToList()
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

        // PUT: /api/services/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateService(Guid id, [FromBody] UpdateServiceRequest request)
        {
            var service = await _db.Services
                .Include(s => s.Components)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (service == null)
                return NotFound(new { error = "Service not found" });

            // basic fields
            service.Name = request.Name;
            service.Category = request.Category;
            service.Description = request.Description;
            service.Price = request.Price;
            service.DurationMinutes = request.DurationMinutes;
            service.UpdatedAt = DateTime.UtcNow;

            // remove all existing components for this service in a single query
            await _db.ServiceComponents
                .Where(sc => sc.ServiceId == service.Id)
                .ExecuteDeleteAsync(); // needs EF Core 7 or newer

            var newComponents = new List<ServiceComponent>();

            if (request.ComponentTypes != null)
            {
                foreach (var typeString in request.ComponentTypes.Distinct())
                {
                    if (Enum.TryParse<ComponentType>(typeString, ignoreCase: true, out var parsed))
                    {
                        newComponents.Add(new ServiceComponent
                        {
                            Id = Guid.NewGuid(),
                            ServiceId = service.Id,
                            ComponentType = parsed
                        });
                    }
                }
            }

            if (newComponents.Count > 0)
            {
                await _db.ServiceComponents.AddRangeAsync(newComponents);
            }

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
                    DurationMinutes = service.DurationMinutes,
                    ComponentTypes = newComponents
                        .Select(c => c.ComponentType.ToString())
                        .ToList()
                }
            });
        }


    }
}

