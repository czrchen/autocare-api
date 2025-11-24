using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace autocare_api.Services
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly IConfiguration _config;
        private readonly ILogger<SmtpEmailSender> _logger;

        public SmtpEmailSender(IConfiguration config, ILogger<SmtpEmailSender> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task SendPasswordResetEmailAsync(string toEmail, string resetLink)
        {
            var section = _config.GetSection("Email:Smtp");
            var from = section["From"];
            var host = section["Host"];
            var portString = section["Port"];
            var user = section["User"];
            var password = section["Password"];

            if (string.IsNullOrWhiteSpace(from) ||
                string.IsNullOrWhiteSpace(host) ||
                string.IsNullOrWhiteSpace(portString) ||
                string.IsNullOrWhiteSpace(user) ||
                string.IsNullOrWhiteSpace(password))
            {
                _logger.LogError("SMTP configuration is missing or incomplete");
                throw new InvalidOperationException("Email configuration is invalid");
            }

            var port = int.Parse(portString);

            using var message = new MailMessage(from, toEmail)
            {
                Subject = "Reset your AutoCare+ password",
                Body = BuildEmailBody(resetLink),
                IsBodyHtml = true
            };

            using var client = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(user, password),
                EnableSsl = true
            };

            try
            {
                await client.SendMailAsync(message);
                _logger.LogInformation("Password reset email sent to {Email}", toEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send password reset email to {Email}", toEmail);
                throw;
            }
        }

        private string BuildEmailBody(string resetLink)
        {
            return $@"
<html>
  <body>
    <p>Hello,</p>
    <p>You requested to reset your AutoCare+ password.</p>
    <p><a href=""{resetLink}"">Click here to reset your password</a></p>
    <p>If you did not request this, you can ignore this email.</p>
    <p>Thanks,<br />AutoCare+ Team</p>
  </body>
</html>";
        }
    }
}
