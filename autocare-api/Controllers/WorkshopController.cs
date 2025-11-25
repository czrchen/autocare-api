using autocare_api.Data;
using autocare_api.DTOs;
using autocare_api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace autocare_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkshopController : ControllerBase
    {
        private readonly AppDbContext _db;

        public WorkshopController(AppDbContext db)
        {
            _db = db;
        }

        // model -> dto
        private static WeeklyOperatingHoursDto MapHoursToDto(WeeklyOperatingHours? hours)
        {
            if (hours == null)
            {
                hours = WeeklyOperatingHours.CreateDefault();
            }

            return new WeeklyOperatingHoursDto
            {
                HoursByDay = hours.HoursByDay
                    .Select(d => new DailyHoursDto
                    {
                        Day = d.Day,
                        IsOpen = d.IsOpen,
                        StartTime = d.StartTime,
                        EndTime = d.EndTime
                    })
                    .ToList()
            };
        }

        // dto -> model
        private static WeeklyOperatingHours MapDtoToHours(WeeklyOperatingHoursDto dto)
        {
            return new WeeklyOperatingHours
            {
                HoursByDay = dto.HoursByDay?
                    .Select(d => new DailyHours
                    {
                        Day = d.Day,
                        IsOpen = d.IsOpen,
                        StartTime = d.StartTime,
                        EndTime = d.EndTime
                    })
                    .ToList()
                    ?? new List<DailyHours>()
            };
        }

        // GET: api/workshop/getAllWorkshops
        [HttpGet("getAllWorkshops")]
        public async Task<IActionResult> GetWorkshops()
        {
            var workshops = await _db.WorkshopProfiles
                .AsNoTracking()
                .ToListAsync();

            var result = workshops.Select(w => new
            {
                id = w.Id,
                name = w.WorkshopName,
                address = w.Address,
                operatingHours = MapHoursToDto(w.OperatingHours),
                rating = w.Rating,
            }).ToList();

            return Ok(result);
        }

        // GET: api/workshop/user/{email}
        [HttpGet("user/{email}")]
        public async Task<IActionResult> GetUserWorkshops(string email)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                return NotFound(new { error = "User not found" });

            var workshops = await _db.WorkshopProfiles
                .Where(w => w.UserId == user.Id)
                .AsNoTracking()
                .ToListAsync();

            var result = workshops.Select(w => new
            {
                Id = w.Id,
                WorkshopName = w.WorkshopName,
                Address = w.Address,
                OperatingHours = MapHoursToDto(w.OperatingHours),
                Rating = w.Rating
            }).ToList();

            return Ok(new
            {
                count = result.Count,
                workshops = result
            });
        }

        // GET: api/workshop/{id}/hours
        [HttpGet("{id:guid}/hours")]
        public async Task<IActionResult> GetOperatingHours(Guid id)
        {
            var workshop = await _db.WorkshopProfiles
                .AsNoTracking()
                .FirstOrDefaultAsync(w => w.Id == id);

            if (workshop == null)
                return NotFound(new { error = "Workshop not found" });

            var dto = MapHoursToDto(workshop.OperatingHours);

            return Ok(dto);
        }

        // PUT: api/workshop/{id}/hours
        [HttpPut("{id:guid}/hours")]
        public async Task<IActionResult> UpdateOperatingHours(
            Guid id,
            [FromBody] WeeklyOperatingHoursDto dto
        )
        {
            if (dto == null)
                return BadRequest(new { error = "Invalid payload" });

            var workshop = await _db.WorkshopProfiles
                .FirstOrDefaultAsync(w => w.Id == id);

            if (workshop == null)
                return NotFound(new { error = "Workshop not found" });

            workshop.OperatingHours = MapDtoToHours(dto);

            await _db.SaveChangesAsync();

            return Ok(new { success = true });
        }
    }
}
