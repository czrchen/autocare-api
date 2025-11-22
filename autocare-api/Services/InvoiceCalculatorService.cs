using autocare_api.Data;
using Microsoft.EntityFrameworkCore;

namespace autocare_api.Services
{
    public class InvoiceCalculatorService
    {
        private readonly AppDbContext _context;
        private readonly decimal TaxRate;

        public InvoiceCalculatorService(AppDbContext context, IConfiguration config)
        {
            _context = context;
            TaxRate = config.GetValue<decimal>("InvoiceSettings:TaxRate");
        }

        public async Task<(decimal subtotal, decimal tax, decimal total)>
            CalculateAsync(Guid serviceRecordId)
        {
            // Get the service record
            var record = await _context.ServiceRecords
                .FirstOrDefaultAsync(x => x.Id == serviceRecordId);

            if (record == null)
                throw new Exception("ServiceRecord not found");

            // Get the service price
            var service = await _context.Services
                .FirstOrDefaultAsync(x => x.Id == record.ServiceId);

            if (service == null)
                throw new Exception("Service not found");

            var subtotal = service.Price;
            var tax = subtotal * TaxRate;
            var total = subtotal + tax;

            return (subtotal, tax, total);
        }
    }
}
