using autocare_api.Data;
using Microsoft.EntityFrameworkCore;

namespace autocare_api.Services
{
    public class InvoiceNumberGeneratorService
    {
        private readonly AppDbContext _context;

        public InvoiceNumberGeneratorService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<string> GenerateInvoiceNumberAsync()
        {
            // Get the highest existing invoice number
            var lastInvoice = await _context.Invoices
                .OrderByDescending(i => i.InvoiceNumber)
                .FirstOrDefaultAsync();

            // If none exist, start at 1
            if (lastInvoice == null || string.IsNullOrWhiteSpace(lastInvoice.InvoiceNumber))
                return "INV-000001";

            // Extract numeric part
            // Expected format: INV-000123
            var parts = lastInvoice.InvoiceNumber.Split('-');
            if (parts.Length != 2 || !int.TryParse(parts[1], out int lastNumber))
                return "INV-000001";  // fallback

            // Increment number
            int nextNumber = lastNumber + 1;

            // Format to 6 digits with padding
            return $"INV-{nextNumber.ToString("D6")}";
        }
    }
}
