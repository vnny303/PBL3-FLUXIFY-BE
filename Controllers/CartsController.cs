using FluxifyAPI.Data;
using FluxifyAPI.DTOs.Cart;
using FluxifyAPI.Mapper;
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
        public async Task<ActionResult<IEnumerable<CartDto>>> GetCarts(Guid tenantId, Guid customerId)
        {
            var carts = await _context.Carts
                .Where(c => c.TenantId == tenantId && c.CustomerId == customerId)
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.ProductSku)
                .ToListAsync();

            return Ok(carts.Select(c => c.ToCartDto()));
        }

        [HttpPost]
        public async Task<ActionResult<CartDto>> CreateCart(Guid tenantId, Guid customerId, [FromBody] CreateCartRequestDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (createDto.TenantId.HasValue && createDto.TenantId.Value != tenantId)
                return BadRequest(new { message = "TenantId trong body không khớp route" });

            if (createDto.CustomerId.HasValue && createDto.CustomerId.Value != customerId)
                return BadRequest(new { message = "CustomerId trong body không khớp route" });

            if (await _context.Customers.FirstOrDefaultAsync(c => c.Id == customerId) == null)
                return NotFound(new { message = "Customer không tồn tại" });

            if (await _context.Tenants.FirstOrDefaultAsync(t => t.Id == tenantId) == null)
                return NotFound(new { message = "Tenant không tồn tại" });

            if (await _context.Carts.FirstOrDefaultAsync(c => c.TenantId == tenantId && c.CustomerId == customerId) != null)
                return BadRequest(new { message = "Customer đã có giỏ hàng" });

            var cart = createDto.ToCartFromCreateDto(tenantId, customerId);

            await _context.Carts.AddAsync(cart);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCarts), new { tenantId, customerId }, cart.ToCartDto());
        }
    }
}
