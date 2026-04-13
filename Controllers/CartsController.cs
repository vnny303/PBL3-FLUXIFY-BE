using FluxifyAPI.DTOs.Cart;
using Microsoft.AspNetCore.Mvc;
using FluxifyAPI.IServices;

namespace FluxifyAPI.Controllers
{
    [Route("api/tenants/{tenantId}/customers/{customerId}/[controller]")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartsController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CartDto>>> GetCarts(Guid tenantId, Guid customerId)
        {
            var result = await _cartService.GetCartsAsync(tenantId, customerId);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            return StatusCode(result.StatusCode, result.Data);
        }

        [HttpPost]
        public async Task<ActionResult<CartDto>> CreateCart(Guid tenantId, Guid customerId, [FromBody] CreateCartRequestDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _cartService.CreateCartAsync(tenantId, customerId, createDto);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            return CreatedAtAction(nameof(GetCarts), new { tenantId, customerId }, result.Data);
        }
    }
}
