using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using autocare_api.Data;
using autocare_api.Models;
using autocare_api.Services;

namespace autocare_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoiceController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly InvoiceNumberGeneratorService _invoiceNumberService;
        private readonly InvoiceCalculatorService _invoiceCalculatorService;
        private readonly InvoicePdfService _pdfService;
        private readonly IEmailSender _emailSender;

        public InvoiceController(
            AppDbContext context,
            InvoiceNumberGeneratorService invoiceNumberService,
            InvoiceCalculatorService invoiceCalculatorService,
            InvoicePdfService pdfService,
            IEmailSender emailSender)
        {
            _context = context;
            _invoiceNumberService = invoiceNumberService;
            _invoiceCalculatorService = invoiceCalculatorService;
            _pdfService = pdfService;
            _emailSender = emailSender;
        }

        // ======================================================
        // GET INVOICE BY ID
        // ======================================================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetInvoice(Guid id)
        {
            var invoice = await _context.Invoices
                .Include(i => i.User)
                .Include(i => i.Workshop)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (invoice == null)
                return NotFound(new { message = "Invoice not found" });

            return Ok(invoice);
        }

        // ======================================================
        // CREATE INVOICE
        // ======================================================
        [HttpPost("create")]
        public async Task<IActionResult> CreateInvoice([FromBody] CreateInvoiceDto dto)
        {
            var serviceRecord = await _context.ServiceRecords
                .FirstOrDefaultAsync(x => x.Id == dto.ServiceRecordId);

            if (serviceRecord == null)
                return NotFound(new { message = "Service record not found" });

            var customer = await _context.Users.FirstOrDefaultAsync(u => u.Id == dto.UserId);
            if (customer == null)
                return NotFound(new { message = "Customer not found" });

            var workshop = await _context.WorkshopProfiles
                .FirstOrDefaultAsync(w => w.Id == dto.WorkshopId);
            if (workshop == null)
                return NotFound(new { message = "Workshop profile not found" });

            var service = await _context.Services
                .FirstOrDefaultAsync(s => s.Id == serviceRecord.ServiceId);
            if (service == null)
                return NotFound(new { message = "Service not found" });

            // Add the main service as a ServiceItem
            var baseItem = new ServiceItem
            {
                Id = Guid.NewGuid(),
                ServiceRecordId = serviceRecord.Id,
                ItemName = service.Name,
                UnitPrice = service.Price,
                Quantity = 1
            };
            _context.ServiceItems.Add(baseItem);
            await _context.SaveChangesAsync();

            // 1. Generate invoice number
            var invoiceNumber = await _invoiceNumberService.GenerateInvoiceNumberAsync();

            // 2. Auto calculate totals (based on ServiceItems)
            var (subtotal, tax, total) =
                await _invoiceCalculatorService.CalculateAsync(dto.ServiceRecordId);

            // 3. Create invoice
            var invoice = new Invoices
            {
                Id = Guid.NewGuid(),
                InvoiceNumber = invoiceNumber,
                UserId = dto.UserId,
                WorkshopId = dto.WorkshopId,
                Subtotal = subtotal,
                Tax = tax,
                Total = total,
                PdfUrl = "",
                CreatedAt = DateTime.UtcNow
            };

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            // 4. Generate PDF (full itemized invoice)
            var pdf = _pdfService.GeneratePdf(
                invoice,
                customer,
                workshop,
                service,
                serviceRecord,
                subtotal,
                tax,
                total
            );

            invoice.PdfUrl = pdf.PublicUrl;
            serviceRecord.InvoiceId = invoice.Id;

            await _context.SaveChangesAsync();
            await _emailSender.SendInvoiceEmailAsync(customer.Email, invoice.InvoiceNumber, pdf.PhysicalPath);
            Console.WriteLine($"Invoice email target: UserId={customer.Id}, Email={customer.Email}");
            Console.WriteLine($"Invoice pdf path: Exists={System.IO.File.Exists(pdf.PhysicalPath)}, Path={pdf.PhysicalPath}");


            return Ok(invoice);
        }

        // ======================================================
        // DELETE INVOICE
        // ======================================================
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteInvoice(Guid id)
        {
            var invoice = await _context.Invoices.FirstOrDefaultAsync(i => i.Id == id);

            if (invoice == null)
                return NotFound(new { message = "Invoice not found" });

            var serviceRecord = await _context.ServiceRecords
                .FirstOrDefaultAsync(s => s.InvoiceId == id);

            if (serviceRecord != null)
            {
                serviceRecord.InvoiceId = null;
                var items = _context.ServiceItems
                    .Where(i => i.ServiceRecordId == serviceRecord.Id)
                    .ToList();

                _context.ServiceItems.RemoveRange(items);
            }

            _context.Invoices.Remove(invoice);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Invoice deleted successfully" });
        }
    }

    // DTO
    public class CreateInvoiceDto
    {
        public Guid ServiceRecordId { get; set; }
        public Guid UserId { get; set; }
        public Guid WorkshopId { get; set; }
    }
}
