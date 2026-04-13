using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FluxifyAPI.DTOs.Cart;
using FluxifyAPI.Services.Interfaces;
using System.Security.Claims;

namespace FluxifyAPI.Controllers
{
    [Authorize(Roles = "customer")]
    [Route("api/tenants/{tenantId}/customers/{customerId}/[controller]")]
    [ApiController]
    public class CartItemsController : ControllerBase
    {
        private readonly ICartItemService _cartItemService;

        public CartItemsController(ICartItemService cartItemService)
        {
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

        // GET: Lấy toàn bộ giỏ hàng của customer
        [HttpGet]
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
        [HttpPost]
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
        [HttpPut("{id}")]
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
        [HttpDelete("{id}")]
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
        [HttpDelete]
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


