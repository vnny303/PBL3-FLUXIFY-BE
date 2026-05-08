using FluxifyAPI.DTOs.Cart;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FluxifyAPI.Services.Interfaces;
using System.Security.Claims;

namespace FluxifyAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }
        [HttpGet]
        public async Task<IActionResult> GetCarts()
        {
            if (!Guid.TryParse(User.FindFirstValue("userId"), out var userId) || !Guid.TryParse(User.FindFirstValue("tenantId"), out var tenantId))
                return Unauthorized(new { message = "Token không hợp lệ" });
            var result = await _cartService.GetCartItemsAsync(tenantId, userId);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });
            return StatusCode(result.StatusCode, result.Data);
        }

        // POST: Thêm sản phẩm vào giỏ hàng (nếu đã có thì cộng dồn số lượng)
        [HttpPost("items")]
        public async Task<IActionResult> AddToCart([FromBody] CreateCartItemRequestDto createDto)
        {
            if (!Guid.TryParse(User.FindFirstValue("userId"), out var userId) || !Guid.TryParse(User.FindFirstValue("tenantId"), out var tenantId))
                return Unauthorized(new { message = "Token không hợp lệ" });
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _cartService.AddToCartAsync(tenantId, userId, createDto);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });
            return StatusCode(result.StatusCode, result.Data);
        }

        // PUT: Cập nhật số lượng của một cart item
        [HttpPut("items/{itemId}")]
        public async Task<IActionResult> UpdateCartItem(Guid itemId, [FromBody] UpdateCartItemRequestDto updateDto)
        {
            if (!Guid.TryParse(User.FindFirstValue("userId"), out var userId) || !Guid.TryParse(User.FindFirstValue("tenantId"), out var tenantId))
                return Unauthorized(new { message = "Token không hợp lệ" });
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _cartService.UpdateCartItemAsync(tenantId, userId, itemId, updateDto);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });
            return StatusCode(result.StatusCode, result.Data);
        }

        // DELETE: Xóa một item khỏi giỏ hàng
        [HttpDelete("items/{itemId}")]
        public async Task<IActionResult> RemoveCartItem(Guid itemId)
        {
            if (!Guid.TryParse(User.FindFirstValue("userId"), out var userId) || !Guid.TryParse(User.FindFirstValue("tenantId"), out var tenantId))
                return Unauthorized(new { message = "Token không hợp lệ" });
            var result = await _cartService.RemoveCartItemAsync(tenantId, userId, itemId);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });
            return StatusCode(result.StatusCode, result.Data);
        }

        // DELETE: Xóa toàn bộ giỏ hàng
        [HttpDelete("items")]
        public async Task<IActionResult> ClearCart()
        {
            if (!Guid.TryParse(User.FindFirstValue("userId"), out var userId) || !Guid.TryParse(User.FindFirstValue("tenantId"), out var tenantId))
                return Unauthorized(new { message = "Token không hợp lệ" });
            var result = await _cartService.ClearCartAsync(tenantId, userId);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });
            return StatusCode(result.StatusCode, result.Data);
        }
    }
}