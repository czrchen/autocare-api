using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using autocare_api.Data;
using autocare_api.Dtos.Workshops;
using autocare_api.Models;
using autocare_api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace autocare_api.Controllers
{
    [ApiController]
    [Route("api/admin/workshops")]
    public class AdminWorkshopsController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<AdminWorkshopsController> _logger;

        public AdminWorkshopsController(
            AppDbContext db,
            IEmailSender emailSender,
            ILogger<AdminWorkshopsController> logger)
        {
            _db = db;
            _emailSender = emailSender;
            _logger = logger;
        }

        // GET api/admin/workshops?status=pending|approved|rejected
        [HttpGet]
        public async Task<ActionResult<List<WorkshopApplicationDto>>> GetWorkshops(
            [FromQuery] string? status
        )
        {
            IQueryable<WorkshopProfile> query = _db.WorkshopProfiles
                .Include(wp => wp.User);

            if (!string.IsNullOrWhiteSpace(status))
            {
                if (!Enum.TryParse<WorkshopApprovalStatus>(
                    status,
                    ignoreCase: true,
                    out var parsedStatus
                ))
                {
                    return BadRequest("Invalid status value");
                }

                query = query.Where(wp => wp.ApprovalStatus == parsedStatus);
            }

            var items = await query
                .OrderByDescending(wp => wp.Id)
                .ToListAsync();

            var result = items.Select(wp => new WorkshopApplicationDto
            {
                Id = wp.Id,
                UserId = wp.UserId,
                OwnerName = wp.User?.FullName ?? "",
                WorkshopName = wp.WorkshopName,
                Email = wp.User?.Email ?? "",
                Phone = wp.User?.Phone ?? "",
                Address = wp.Address,
                OperatingHours = wp.OperatingHours,
                Status = wp.ApprovalStatus.ToString().ToLowerInvariant(),
                CreatedAt = wp.User?.CreatedAt ?? DateTime.UtcNow,
                ApprovalNotes = wp.ApprovalNotes
            }).ToList();

            return Ok(result);
        }

        // POST api/admin/workshops/{id}/approve
        [HttpPost("{id:guid}/approve")]
        public async Task<IActionResult> ApproveWorkshop(
            Guid id,
            [FromBody] WorkshopApprovalRequest request
        )
        {
            var workshop = await _db.WorkshopProfiles
                .Include(wp => wp.User)
                .FirstOrDefaultAsync(wp => wp.Id == id);

            if (workshop == null)
                return NotFound();

            if (workshop.ApprovalStatus == WorkshopApprovalStatus.Approved)
                return BadRequest("Workshop already approved");

            var adminId = GetCurrentUserId();

            workshop.ApprovalStatus = WorkshopApprovalStatus.Approved;
            workshop.ApprovalNotes = request.Notes;
            workshop.ReviewedByAdminId = adminId;
            workshop.ReviewedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            var toEmail = workshop.User?.Email;
            if (!string.IsNullOrWhiteSpace(toEmail))
            {
                var subject = "AutoCare+ Workshop Application Approved";
                var notes = (request.Notes ?? "").Trim();

                var body =
                    $"Hello {workshop.User?.FullName ?? "Workshop Owner"},\n\n" +
                    $"Your workshop application for \"{workshop.WorkshopName}\" has been approved.\n\n" +
                    (string.IsNullOrWhiteSpace(notes) ? "" : $"Message from admin:\n{notes}\n\n") +
                    "AutoCare+";

                try
                {
                    await _emailSender.SendPlainTextAsync(toEmail, subject, body);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Approve email failed for workshop {WorkshopId} to {Email}", workshop.Id, toEmail);
                    // Keep approval successful even if email fails
                }
            }

            return NoContent();
        }

        // POST api/admin/workshops/{id}/reject
        [HttpPost("{id:guid}/reject")]
        public async Task<IActionResult> RejectWorkshop(
            Guid id,
            [FromBody] WorkshopApprovalRequest request
        )
        {
            if (string.IsNullOrWhiteSpace(request.Notes))
                return BadRequest("Rejection notes are required");

            var workshop = await _db.WorkshopProfiles
                .Include(wp => wp.User)
                .FirstOrDefaultAsync(wp => wp.Id == id);

            if (workshop == null)
                return NotFound();

            var adminId = GetCurrentUserId();

            workshop.ApprovalStatus = WorkshopApprovalStatus.Rejected;
            workshop.ApprovalNotes = request.Notes;
            workshop.ReviewedByAdminId = adminId;
            workshop.ReviewedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            var toEmail = workshop.User?.Email;
            if (!string.IsNullOrWhiteSpace(toEmail))
            {
                var subject = "AutoCare+ Workshop Application Rejected";
                var notes = request.Notes.Trim();

                var body =
                    $"Hello {workshop.User?.FullName ?? "Workshop Owner"},\n\n" +
                    $"Your workshop application for \"{workshop.WorkshopName}\" has been rejected.\n\n" +
                    $"Message from admin:\n{notes}\n\n" +
                    "AutoCare+";

                try
                {
                    await _emailSender.SendPlainTextAsync(toEmail, subject, body);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Reject email failed for workshop {WorkshopId} to {Email}", workshop.Id, toEmail);
                    // Keep rejection successful even if email fails
                }
            }

            return NoContent();
        }

        private Guid? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(userIdClaim, out var id))
                return id;

            return null;
        }
    }
}
