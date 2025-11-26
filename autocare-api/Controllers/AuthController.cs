using autocare_api.Data;
using autocare_api.DTOs;
using autocare_api.Models;
using autocare_api.Services;
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
        private readonly IConfiguration _config;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<AuthController> _logger;
        private readonly IGeocodingService _geocodingService;

        public AuthController(
            AppDbContext db,
            IConfiguration config,
            IEmailSender emailSender,
            ILogger<AuthController> logger,
            IGeocodingService geocodingService)
        {
            _db = db;
            _config = config;
            _emailSender = emailSender;
            _logger = logger;
            _geocodingService = geocodingService;
        }

        [HttpPost("register-driver")]
        public async Task<IActionResult> Register(DriverRegisterRequest request)
        {
            var existing = await _db.Users.FirstOrDefaultAsync(x => x.Email == request.Email);
            if (existing != null)
            {
                return BadRequest(new { error = "Email already exists" });
            }

            string passwordHash = HashPassword(request.Password);

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

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return Ok(new { success = true, message = "User registered successfully" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginRequest request)
        {
            var user = await _db.Users
                .FirstOrDefaultAsync(x => x.Email == request.Email);

            if (user == null)
            {
                return Unauthorized(new { error = "Invalid credentials" });
            }

            string hashed = HashPassword(request.Password);

            if (user.PasswordHash != hashed)
            {
                return Unauthorized(new { error = "Invalid credentials" });
            }

            return Ok(new
            {
                success = true,
                user = new
                {
                    user.Id,
                    FullName = user.FullName,
                    user.Email,
                    user.Phone,
                    user.Role
                }
            });
        }

        [HttpPost("register-workshop")]
        public async Task<IActionResult> RegisterWorkshop(WorkshopRegisterRequest request)
        {
            Console.WriteLine("=== REQUEST DEBUG ===");
            Console.WriteLine($"OperatingHours is null? {request.OperatingHours == null}");
            Console.WriteLine($"OperatingHours :  {request.OperatingHours}");
            Console.WriteLine($"HoursByDay is null? {request.OperatingHours?.HoursByDay == null}");
            Console.WriteLine($"HoursByDay Count: {request.OperatingHours?.HoursByDay?.Count ?? 0}");

            var serialized = System.Text.Json.JsonSerializer.Serialize(request.OperatingHours);
            Console.WriteLine($"Received OperatingHours JSON: {serialized}");

            var existing = await _db.Users.FirstOrDefaultAsync(x => x.Email == request.Email);
            if (existing != null)
            {
                return BadRequest(new { error = "Email already exists" });
            }

            string passwordHash = HashPassword(request.Password);

            var user = new User
            {
                Id = Guid.NewGuid(),
                FullName = request.OwnerName,
                Email = request.Email,
                Phone = request.Phone,
                PasswordHash = passwordHash,
                Role = "Workshop",
                CreatedAt = DateTime.UtcNow
            };

            // Create workshop profile
            var workshop = new WorkshopProfile
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                WorkshopName = request.WorkshopName,
                Address = request.Address,
                OperatingHours = request.OperatingHours,
                Rating = 0,
                ApprovalStatus = WorkshopApprovalStatus.Pending
            };

            // Geocode the address and set latitude and longitude if found
            try
            {
                var coords = await _geocodingService.GeocodeAsync(request.Address);

                if (coords != null)
                {
                    workshop.Latitude = coords.Value.Latitude;
                    workshop.Longitude = coords.Value.Longitude;
                }
            }
            catch (Exception ex)
            {
                // Log the error but do not block registration
                _logger.LogError(ex, "Geocoding failed for workshop {WorkshopName}", request.WorkshopName);
            }

            _db.Users.Add(user);
            _db.WorkshopProfiles.Add(workshop);
            await _db.SaveChangesAsync();

            return Ok(new { success = true, message = "Workshop registered successfully" });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
            {
                return Ok(new { message = "If an account exists, a reset link has been sent." });
            }

            var rawToken = GenerateResetToken();
            var tokenHash = HashString(rawToken);
            var expiresAt = DateTime.UtcNow.AddHours(24);

            var oldTokens = await _db.PasswordResetTokens
                .Where(t => t.UserId == user.Id &&
                            t.UsedAt == null &&
                            t.ExpiresAt > DateTime.UtcNow)
                .ToListAsync();

            foreach (var t in oldTokens)
            {
                t.ExpiresAt = DateTime.UtcNow;
            }

            var resetToken = new PasswordResetToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                TokenHash = tokenHash,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = expiresAt
            };

            _db.PasswordResetTokens.Add(resetToken);
            await _db.SaveChangesAsync();

            var frontendBaseUrl = _config["FrontendBaseUrl"] ?? "http://localhost:3000";

            var encodedEmail = Uri.EscapeDataString(user.Email);
            var encodedToken = Uri.EscapeDataString(rawToken);

            var resetLink = $"{frontendBaseUrl}/reset-password?email={encodedEmail}&token={encodedToken}";

            try
            {
                await _emailSender.SendPasswordResetEmailAsync(user.Email, resetLink);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending password reset email to {Email}", user.Email);
            }

            return Ok(new { message = "If an account exists, a reset link has been sent." });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
            {
                return Ok(new { message = "If the link is valid, the password has been reset." });
            }

            var providedTokenHash = HashString(request.Token);

            var tokenRecord = await _db.PasswordResetTokens
                .FirstOrDefaultAsync(t =>
                    t.UserId == user.Id &&
                    t.TokenHash == providedTokenHash &&
                    t.UsedAt == null &&
                    t.ExpiresAt > DateTime.UtcNow);

            if (tokenRecord == null)
            {
                return BadRequest(new { message = "Reset link is invalid or has expired." });
            }

            var newPasswordHash = HashPassword(request.NewPassword);
            user.PasswordHash = newPasswordHash;

            tokenRecord.UsedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return Ok(new { message = "Password has been reset successfully." });
        }

        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        private string HashString(string value)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(value);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        private string GenerateResetToken()
        {
            var bytes = RandomNumberGenerator.GetBytes(32);
            return Convert.ToBase64String(bytes);
        }
    }
}
