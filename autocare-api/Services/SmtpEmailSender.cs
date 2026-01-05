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

        public async Task SendInvoiceEmailAsync(string toEmail, string invoiceNumber, string pdfFilePath)
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

            if (!File.Exists(pdfFilePath))
                throw new FileNotFoundException("Invoice PDF not found", pdfFilePath);

            var port = int.Parse(portString);

            using var message = new MailMessage(from, toEmail)
            {
                Subject = $"AutoCare+ Invoice {invoiceNumber}",
                Body = BuildInvoiceEmailBody(invoiceNumber),
                IsBodyHtml = true
            };

            message.Attachments.Add(new Attachment(pdfFilePath));

            using var client = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(user, password),
                EnableSsl = true
            };

            try
            {
                await client.SendMailAsync(message);
                _logger.LogInformation("Invoice email sent to {Email} (Invoice {InvoiceNumber})", toEmail, invoiceNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send invoice email to {Email} (Invoice {InvoiceNumber})", toEmail, invoiceNumber);
                throw;
            }
        }

        public async Task SendPlainTextAsync(string toEmail, string subject, string body)
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

            if (string.IsNullOrWhiteSpace(toEmail))
                throw new ArgumentException("Recipient email is required", nameof(toEmail));

            if (string.IsNullOrWhiteSpace(subject))
                subject = "AutoCare+ Notification";

            var port = int.Parse(portString);

            using var message = new MailMessage(from, toEmail)
            {
                Subject = subject,
                Body = body ?? string.Empty,
                IsBodyHtml = false
            };

            using var client = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(user, password),
                EnableSsl = true
            };

            try
            {
                await client.SendMailAsync(message);
                _logger.LogInformation("Plain text email sent to {Email}", toEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send plain text email to {Email}", toEmail);
                throw;
            }
        }

        private string BuildInvoiceEmailBody(string invoiceNumber)
        {
            return $@"
<html>
  <body>
    <p>Hello,</p>
    <p>Your invoice <b>{invoiceNumber}</b> is attached to this email.</p>
    <p>Thank you for choosing AutoCare+.</p>
    <p>Regards,<br/>AutoCare+ Team</p>
  </body>
</html>";
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
