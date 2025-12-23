using autocare_api.Data;
using autocare_api.DTOs;
using autocare_api.DTOs.Response;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace autocare_api.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    public class NotificationsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IAmazonSimpleNotificationService _sns;
        private readonly IConfiguration _config;

        public NotificationsController(
            AppDbContext context,
            IAmazonSimpleNotificationService sns,
            IConfiguration config)
        {
            _context = context;
            _sns = sns;
            _config = config;
        }

        // --------------------------------------------------
        // Get current notification status
        // --------------------------------------------------
        [HttpPost("status")]
        public async Task<ActionResult<NotificationStatusResponse>> GetStatus(
            [FromBody] NotificationStatusRequest dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null)
                return NotFound();

            return Ok(new NotificationStatusResponse
            {
                Requested = user.EmailNotificationsRequested,
                Confirmed = user.EmailNotificationsConfirmed
            });
        }

        // --------------------------------------------------
        // Subscribe user to SNS email notifications
        // --------------------------------------------------
        [HttpPost("subscribe")]
        public async Task<IActionResult> Subscribe(
            [FromBody] NotificationStatusRequest dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null)
                return NotFound();

            var topicArn = _config["AWS:SnsTopicArn"];

            if (string.IsNullOrWhiteSpace(topicArn))
                return StatusCode(500, "SNS Topic ARN is not configured.");

            // Request SNS subscription
            var request = new SubscribeRequest
            {
                TopicArn = topicArn,
                Protocol = "email",
                Endpoint = user.Email
            };

            await _sns.SubscribeAsync(request);

            // Update local state
            user.EmailNotificationsRequested = true;
            user.EmailNotificationsConfirmed = false;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Subscription requested. Please confirm via the email sent by AWS SNS."
            });
        }

        // --------------------------------------------------
        // Unsubscribe user (local state only - Phase 1)
        // --------------------------------------------------
        [HttpPost("unsubscribe")]
        public async Task<IActionResult> Unsubscribe(
            [FromBody] NotificationStatusRequest dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null)
                return NotFound();

            user.EmailNotificationsRequested = false;
            user.EmailNotificationsConfirmed = false;

            // NOTE:
            // Proper SNS unsubscribe requires SubscriptionArn.
            // This is acceptable for Phase 1.

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Unsubscribed successfully."
            });
        }
    }
}
