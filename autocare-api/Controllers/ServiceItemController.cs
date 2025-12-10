using Microsoft.AspNetCore.Mvc;
using autocare_api.Data;
using autocare_api.Models;
using Microsoft.EntityFrameworkCore;

namespace autocare_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceItemController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ServiceItemController(AppDbContext context)
        {
            _context = context;
        }

        // ===========================================
        // ADD SERVICE ITEMS TO RECORD
        // ===========================================
        [HttpPost("add")]
        public async Task<IActionResult> AddItems([FromBody] AddItemsDto dto)
        {
            if (dto.Items == null || dto.Items.Count == 0)
                return BadRequest(new { message = "No items provided" });

            foreach (var item in dto.Items)
            {
                _context.ServiceItems.Add(new ServiceItem
                {
                    Id = Guid.NewGuid(),
                    ServiceRecordId = dto.ServiceRecordId,
                    ItemName = item.ItemName,
                    UnitPrice = item.UnitPrice,
                    Quantity = item.Quantity
                });
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Items saved" });
        }

        // GET: /api/serviceitem/by-record/{serviceRecordId}
        [HttpGet("by-record/{serviceRecordId}")]
        public async Task<IActionResult> GetItemsByRecord(Guid serviceRecordId)
        {
            var items = await _context.ServiceItems
                .Where(i => i.ServiceRecordId == serviceRecordId)
                .ToListAsync();

            return Ok(items);
        }

    }

    public class AddItemsDto
    {
        public Guid ServiceRecordId { get; set; }
        public List<ServiceItemDto> Items { get; set; } = new();
    }

    public class ServiceItemDto
    {
        public string ItemName { get; set; } = "";
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
    }
}
