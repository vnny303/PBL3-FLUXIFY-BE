using FluxifyAPI.DTOs.Cart;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FluxifyAPI.Services.Interfaces;
using System.Security.Claims;

namespace FluxifyAPI.Controllers
{
    [Authorize]
    [Route("api/tenants/{tenantId}/customers/{customerId}/[controller]")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly ICartItemService _cartItemService;

        public CartsController(ICartService cartService, ICartItemService cartItemService)
        {
            _cartService = cartService;
            _cartItemService = cartItemService;
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
            if (!Guid.TryParse(User.FindFirstValue("userId"), out var userId))
                return Unauthorized(new { message = "Token không hợp lệ" });
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

        // GET: Lấy toàn bộ giỏ hàng của customer
        [HttpGet("items")]
        public async Task<IActionResult> GetCartItems(Guid tenantId, Guid customerId)
        {
            if (!TryValidateCustomerScope(customerId, out var errorResult))
                return errorResult!;

            var result = await _cartItemService.GetCartItemsAsync(tenantId, customerId);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            return StatusCode(result.StatusCode, result.Data);
        }

        // POST: Thêm sản phẩm vào giỏ hàng (nếu đã có thì cộng dồn số lượng)
        [HttpPost("items")]
        public async Task<IActionResult> AddToCart(Guid tenantId, Guid customerId, [FromBody] CreateCartItemRequestDto createDto)
        {
            if (!TryValidateCustomerScope(customerId, out var errorResult))
                return errorResult!;

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _cartItemService.AddToCartAsync(tenantId, customerId, createDto);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            return StatusCode(result.StatusCode, result.Data);
        }

        // PUT: Cập nhật số lượng của một cart item
        [HttpPut("items/{id}")]
        public async Task<IActionResult> UpdateCartItem(Guid tenantId, Guid customerId, Guid id, [FromBody] UpdateCartItemRequestDto updateDto)
        {
            if (!TryValidateCustomerScope(customerId, out var errorResult))
                return errorResult!;

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _cartItemService.UpdateCartItemAsync(tenantId, customerId, id, updateDto);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            return StatusCode(result.StatusCode, result.Data);
        }

        // DELETE: Xóa một item khỏi giỏ hàng
        [HttpDelete("items/{id}")]
        public async Task<IActionResult> RemoveCartItem(Guid tenantId, Guid customerId, Guid id)
        {
            if (!TryValidateCustomerScope(customerId, out var errorResult))
                return errorResult!;

            var result = await _cartItemService.RemoveCartItemAsync(tenantId, customerId, id);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            return StatusCode(result.StatusCode, result.Data);
        }

        // DELETE: Xóa toàn bộ giỏ hàng
        [HttpDelete("items")]
        public async Task<IActionResult> ClearCart(Guid tenantId, Guid customerId)
        {
            if (!TryValidateCustomerScope(customerId, out var errorResult))
                return errorResult!;

            var result = await _cartItemService.ClearCartAsync(tenantId, customerId);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            return StatusCode(result.StatusCode, result.Data);
        }
    }
}


