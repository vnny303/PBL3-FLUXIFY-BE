using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FluxifyAPI.Data;
using FluxifyAPI.DTOs.Order;
using FluxifyAPI.Mapper;
using FluxifyAPI.Models;

namespace FluxifyAPI.Controllers
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
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders(Guid tenantId)
        {
            var orders = await _context.Orders
                .Where(o => o.TenantId == tenantId)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.ProductSku)
                        .ThenInclude(s => s.Product)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return Ok(orders.Select(o => o.ToOrderDto()));
        }

        // GET: api/tenants/{tenantId}/orders/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrder(Guid tenantId, Guid id)
        {
            var order = await _context.Orders
                .Where(o => o.TenantId == tenantId && o.Id == id)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.ProductSku)
                        .ThenInclude(s => s.Product)
                .Include(o => o.Customer)
                .FirstOrDefaultAsync();

            if (order == null)
            {
                return NotFound();
            }

            return Ok(order.ToOrderDto());
        }

        // POST: api/tenants/{tenantId}/orders
        [HttpPost]
        public async Task<ActionResult<OrderDto>> CreateOrder(Guid tenantId, [FromBody] CreateOrderRequestDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (createDto.CustomerId.HasValue)
            {
                var customerExists = await _context.Customers.AnyAsync(c => c.Id == createDto.CustomerId.Value && c.TenantId == tenantId);
                if (!customerExists)
                    return BadRequest(new { message = "Customer không tồn tại trong tenant này" });
            }

            var order = createDto.ToOrderFromCreateDto(tenantId);

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrder), new { tenantId, id = order.Id }, order.ToOrderDto());
        }

        // PUT: api/tenants/{tenantId}/orders/{id}/status
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(Guid tenantId, Guid id, [FromBody] UpdateOrderStatusRequestDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.TenantId == tenantId && o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            updateDto.ToOrderFromUpdateStatusDto(order);
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