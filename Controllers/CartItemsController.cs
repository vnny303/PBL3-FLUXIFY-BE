using Microsoft.AspNetCore.Mvc;
using FluxifyAPI.DTOs.Cart;
using FluxifyAPI.IServices;

namespace FluxifyAPI.Controllers
{
    [Route("api/tenants/{tenantId}/customers/{customerId}/[controller]")]
    [ApiController]
    public class CartItemsController : ControllerBase
    {
        private readonly ICartItemService _cartItemService;

        public CartItemsController(ICartItemService cartItemService)
        {
            _cartItemService = cartItemService;
        }

        // GET: Lấy toàn bộ giỏ hàng của customer
        [HttpGet]
        public async Task<ActionResult> GetCartItems(Guid tenantId, Guid customerId)
        {
            var result = await _cartItemService.GetCartItemsAsync(tenantId, customerId);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            return StatusCode(result.StatusCode, result.Data);
        }

        // POST: Thêm sản phẩm vào giỏ hàng (nếu đã có thì cộng dồn số lượng)
        [HttpPost]
        public async Task<ActionResult> AddToCart(Guid tenantId, Guid customerId, [FromBody] CreateCartItemRequestDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _cartItemService.AddToCartAsync(tenantId, customerId, createDto);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            return StatusCode(result.StatusCode, result.Data);
        }

        // PUT: Cập nhật số lượng của một cart item
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateCartItem(Guid tenantId, Guid customerId, Guid id, [FromBody] UpdateCartItemRequestDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _cartItemService.UpdateCartItemAsync(tenantId, customerId, id, updateDto);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            return StatusCode(result.StatusCode, result.Data);
        }

        // DELETE: Xóa một item khỏi giỏ hàng
        [HttpDelete("{id}")]
        public async Task<ActionResult> RemoveCartItem(Guid tenantId, Guid customerId, Guid id)
        {
            var result = await _cartItemService.RemoveCartItemAsync(tenantId, customerId, id);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            return StatusCode(result.StatusCode, result.Data);
        }

        // DELETE: Xóa toàn bộ giỏ hàng
        [HttpDelete]
        public async Task<ActionResult> ClearCart(Guid tenantId, Guid customerId)
        {
            var result = await _cartItemService.ClearCartAsync(tenantId, customerId);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            return StatusCode(result.StatusCode, result.Data);
        }
    }
}
