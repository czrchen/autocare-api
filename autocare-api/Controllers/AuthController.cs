using autocare_api.Data;
using autocare_api.DTOs;
using autocare_api.Models;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace autocare_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;

        public AuthController(AppDbContext db)
        {
            _db = db;
        }

        [HttpPost("register-driver")]
        public async Task<IActionResult> Register(DriverRegisterRequest request)
        {
            // 1. Validate Email Existence
            var existing = await _db.Users.FirstOrDefaultAsync(x => x.Email == request.Email);
            if (existing != null)
            {
                return BadRequest(new { error = "Email already exists" });
            }

            // 2. Hash Password
            string passwordHash = HashPassword(request.Password);

            // 3. Create User
            var user = new User
            {
                Id = Guid.NewGuid(),
                FullName = request.Name,
                Email = request.Email,
                Phone = request.Phone,
                PasswordHash = passwordHash,
                Role = "Driver",
                CreatedAt = DateTime.UtcNow
            };

            // 4. Save to DB
            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return Ok(new { success = true, message = "User registered successfully" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginRequest request)
        {
            // 1. Find user by email AND role together
            var user = await _db.Users
                .FirstOrDefaultAsync(x =>
                    x.Email == request.Email &&
                    x.Role.ToLower() == request.Role.ToLower());

            if (user == null)
                return Unauthorized(new { error = "Invalid credentials" });

            // 2. Check password
            string hashed = HashPassword(request.Password);

            if (user.PasswordHash != hashed)
                return Unauthorized(new { error = "Invalid credentials" });

            // 3. Success: return user info
            return Ok(new
            {
                success = true,
                user = new
                {
                    user.Id,
                    user.FullName,
                    user.Email,
                    user.Phone,
                    user.Role
                }
            });
        }

        [HttpPost("register-workshop")]
        public async Task<IActionResult> RegisterWorkshop(WorkshopRegisterRequest request)
        {
            // 1. Check if email already exists
            var existing = await _db.Users.FirstOrDefaultAsync(x => x.Email == request.Email);
            if (existing != null)
            {
                return BadRequest(new { error = "Email already exists" });
            }

            // 2. Hash password
            string passwordHash = HashPassword(request.Password);

            // 3. Create User (WORKSHOP ACCOUNT)
            var user = new User
            {
                Id = Guid.NewGuid(),
                FullName = request.OwnerName,          // Save owner name here
                Email = request.Email,
                Phone = request.Phone,
                PasswordHash = passwordHash,
                Role = "Workshop",                     // Important
                CreatedAt = DateTime.UtcNow
            };

            // 4. Create WorkshopProfile
            var workshop = new WorkshopProfile
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                WorkshopName = request.WorkshopName,
                Address = request.Address,
                OperatingHours = request.OperatingHours,
                Rating = 0                             // default rating = 0
            };

            // 5. Save to DB
            _db.Users.Add(user);
            _db.WorkshopProfiles.Add(workshop);
            await _db.SaveChangesAsync();

            return Ok(new { success = true, message = "Workshop registered successfully" });
        }
        

        // 🔐 Simple SHA256 hashing
        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
