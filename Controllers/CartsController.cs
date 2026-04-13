using FluxifyAPI.DTOs.Cart;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FluxifyAPI.Services.Interfaces;
using System.Security.Claims;

namespace FluxifyAPI.Controllers
{
    [Authorize(Roles = "customer")]
    [Route("api/tenants/{tenantId}/customers/{customerId}/[controller]")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartsController(ICartService cartService)
        {
            _cartService = cartService;
        }

        private bool TryValidateCustomerScope(Guid customerId, out IActionResult? errorResult)
        {
            errorResult = null;
            var userIdClaim = User.FindFirstValue("userId");
            if (!Guid.TryParse(userIdClaim, out var tokenCustomerId))
            {
                errorResult = Unauthorized(new { message = "Token không hợp lệ" });
                return false;
            }

            if (tokenCustomerId != customerId)
            {
                errorResult = StatusCode(403, new { message = "Bạn không có quyền truy cập giỏ hàng này" });
                return false;
            }

            return true;
        }

        [HttpGet]
        public async Task<IActionResult> GetCarts(Guid tenantId, Guid customerId)
        {
            if (!TryValidateCustomerScope(customerId, out var errorResult))
                return errorResult!;

            var result = await _cartService.GetCartsAsync(tenantId, customerId);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            return StatusCode(result.StatusCode, result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCart(Guid tenantId, Guid customerId, [FromBody] CreateCartRequestDto createDto)
        {
            if (!TryValidateCustomerScope(customerId, out var errorResult))
                return errorResult!;

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _cartService.CreateCartAsync(tenantId, customerId, createDto);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            return CreatedAtAction(nameof(GetCarts), new { tenantId, customerId }, result.Data);
        }
    }
}


