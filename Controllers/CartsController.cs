using FluxifyAPI.Data;
using FluxifyAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FluxifyAPI.Controllers
{
    [Route("api/tenants/{tenantId}/customers/{customerId}/[controller]")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CartsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetCarts(Guid tenantId, Guid customerId)
        {
            var carts = await _context.Carts
                .Where(c => c.TenantId == tenantId && c.CustomerId == customerId)
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.ProductSku)
                .ToListAsync();
            return Ok(carts);
        }
        [HttpPost]
        public async Task<IActionResult> CreateCart(Guid tenantId, Guid customerId)
        {
            var cart = new Cart
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                CustomerId = customerId
            };
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (await _context.Customers.FirstOrDefaultAsync(c => c.Id == customerId) == null)
                return NotFound(new { message = "Customer không tồn tại" });
            if (await _context.Tenants.FirstOrDefaultAsync(t => t.Id == tenantId) == null)
                return NotFound(new { message = "Tenant không tồn tại" });
            if (await _context.Carts.FirstOrDefaultAsync(c => c.TenantId == tenantId && c.CustomerId == customerId) != null)
                return BadRequest(new { message = "Customer đã có giỏ hàng" });
            await _context.Carts.AddAsync(cart);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCarts), new { tenantId, customerId }, cart);
        }
    }
}
