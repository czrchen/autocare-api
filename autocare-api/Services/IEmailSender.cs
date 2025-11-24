namespace autocare_api.Services
{
    public interface IEmailSender
    {
        Task SendPasswordResetEmailAsync(string toEmail, string resetLink);
    }
}
