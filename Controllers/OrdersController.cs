using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FluxifyAPI.DTOs.Order;
using FluxifyAPI.Helpers;
using FluxifyAPI.Services.Interfaces;
using System.Security.Claims;

namespace FluxifyAPI.Controllers
{
    [Authorize(Roles = "merchant")]
    [Route("api/tenants/{tenantId}/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // GET: api/tenants/{tenantId}/orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders(Guid tenantId, [FromQuery] QueryOrder query)
        {
            if (!Guid.TryParse(User.FindFirstValue("userId"), out var userId))
                return Unauthorized(new { message = "Token không hợp lệ hoặc thiếu userId claim" });

            var result = await _orderService.GetOrdersAsync(tenantId, userId, query);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            return StatusCode(result.StatusCode, result.Data);
        }

        // GET: api/tenants/{tenantId}/orders/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrder(Guid tenantId, Guid id)
        {
            if (!Guid.TryParse(User.FindFirstValue("userId"), out var userId))
                return Unauthorized(new { message = "Token không hợp lệ hoặc thiếu userId claim" });

            var result = await _orderService.GetOrderAsync(tenantId, userId, id);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            return StatusCode(result.StatusCode, result.Data);
        }

        // POST: api/tenants/{tenantId}/orders
        [HttpPost]
        public async Task<ActionResult<OrderDto>> CreateOrder(Guid tenantId, [FromBody] CreateOrderRequestDto createDto)
        {
            if (!Guid.TryParse(User.FindFirstValue("userId"), out var userId))
                return Unauthorized(new { message = "Token không hợp lệ hoặc thiếu userId claim" });
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _orderService.CreateOrderAsync(tenantId, userId, createDto);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            return CreatedAtAction(nameof(GetOrder), new { tenantId, id = result.Data!.Id }, result.Data);
        }

        // PUT: api/tenants/{tenantId}/orders/{id}/status
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(Guid tenantId, Guid id, [FromBody] UpdateOrderStatusRequestDto updateDto)
        {
            if (!Guid.TryParse(User.FindFirstValue("userId"), out var userId))
                return Unauthorized(new { message = "Token không hợp lệ hoặc thiếu userId claim" });
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _orderService.UpdateOrderStatusAsync(tenantId, userId, id, updateDto);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            if (result.StatusCode == 204)
                return NoContent();

            return StatusCode(result.StatusCode, result.Data);
        }

        // DELETE: api/tenants/{tenantId}/orders/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(Guid tenantId, Guid id)
        {
            if (!Guid.TryParse(User.FindFirstValue("userId"), out var userId))
                return Unauthorized(new { message = "Token không hợp lệ hoặc thiếu userId claim" });

            var result = await _orderService.DeleteOrderAsync(tenantId, userId, id);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            if (result.StatusCode == 204)
                return NoContent();

            return StatusCode(result.StatusCode, result.Data);
        }
    }
}

