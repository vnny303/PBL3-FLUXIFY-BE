using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopifyAPI.Data;
using ShopifyAPI.Models;

namespace ShopifyAPI.Controllers
{
    [Route("api/tenants/{tenantId}/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrdersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/tenants/{tenantId}/orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders(Guid tenantId)
        {
            return await _context.Orders
                .Where(o => o.TenantId == tenantId)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        // GET: api/tenants/{tenantId}/orders/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(Guid tenantId, Guid id)
        {
            var order = await _context.Orders
                .Where(o => o.TenantId == tenantId && o.Id == id)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.Customer)
                .FirstOrDefaultAsync();

            if (order == null)
            {
                return NotFound();
            }

            return order;
        }

        // POST: api/tenants/{tenantId}/orders
        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder(Guid tenantId, Order order)
        {
            order.Id = Guid.NewGuid();
            order.TenantId = tenantId;
            order.CreatedAt = DateTime.Now;
            order.Status = "Pending";
            order.PaymentStatus = "Pending";

            // Tính tổng tiền
            decimal total = 0;
            foreach (var item in order.OrderItems)
            {
                item.Id = Guid.NewGuid();
                item.OrderId = order.Id;
                total += item.UnitPrice * item.Quantity;
            }
            order.TotalAmount = total;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrder), new { tenantId, id = order.Id }, order);
        }

        // PUT: api/tenants/{tenantId}/orders/{id}/status
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(Guid tenantId, Guid id, [FromBody] string status)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.TenantId == tenantId && o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            order.Status = status;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/tenants/{tenantId}/orders/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(Guid tenantId, Guid id)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.TenantId == tenantId && o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}