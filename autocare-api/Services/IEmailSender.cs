namespace autocare_api.Services
{
    public interface IEmailSender
    {
        Task SendPasswordResetEmailAsync(string toEmail, string resetLink);

        Task SendInvoiceEmailAsync(string toEmail, string invoiceNumber, string pdfFilePath);
    }
}
