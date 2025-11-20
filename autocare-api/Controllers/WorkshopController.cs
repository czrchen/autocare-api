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

    public class WorkshopController : ControllerBase
    {
        private readonly AppDbContext _db;
        public WorkshopController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet("getAllWorkshops")]
        public async Task<IActionResult> GetWorkshops()
        {
            var workshops = await _db.WorkshopProfiles
                .Select(w => new {
                    id = w.Id,
                    name = w.WorkshopName,
                    address = w.Address,
                    hours = w.OperatingHours,
                    rating = w.Rating,
                })
                .ToListAsync();

            return Ok(workshops);
        }

        [HttpGet("user/{email}")]
        public async Task<IActionResult> GetUserWorkshops(string email)
        {
            // 1️⃣ Find user by email
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                return NotFound(new { error = "User not found" });

            // 2️⃣ Get workshops assigned to this user
            var workshops = await _db.WorkshopProfiles
                .Where(w => w.UserId == user.Id)
                .Select(w => new
                {
                    Id = w.Id,
                    WorkshopName = w.WorkshopName,
                    Address = w.Address,
                    OperatingHours = w.OperatingHours,
                    Rating = w.Rating
                })
                .ToListAsync();

            // 3️⃣ Return formatted response
            return Ok(new
            {
                count = workshops.Count,
                workshops = workshops
            });
        }

    }
}
