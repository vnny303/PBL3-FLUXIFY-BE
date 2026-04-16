using FluxifyAPI.DTOs.Order;
using FluxifyAPI.Helpers;
using FluxifyAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FluxifyAPI.Controllers
{
    [Authorize(Roles = "customer")]
    [Route("api/customer/orders")]
    [ApiController]
    public class CustomerOrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public CustomerOrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyOrders([FromQuery] QueryOrder query)
        {
            if (!Guid.TryParse(User.FindFirstValue("userId"), out var customerId) ||
                !Guid.TryParse(User.FindFirstValue("tenantId"), out var tenantId))
                return Unauthorized(new { message = "Token không hợp lệ" });

            query.CustomerId = customerId;
            var result = await _orderService.GetMyOrdersAsync(tenantId, customerId, query);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            return StatusCode(result.StatusCode, result.Data);
        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetMyOrder(Guid orderId)
        {
            if (!Guid.TryParse(User.FindFirstValue("userId"), out var customerId) ||
                !Guid.TryParse(User.FindFirstValue("tenantId"), out var tenantId))
                return Unauthorized(new { message = "Token không hợp lệ" });

            var result = await _orderService.GetMyOrderAsync(tenantId, customerId, orderId);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            return StatusCode(result.StatusCode, result.Data);
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutOrderRequestDto checkoutDto)
        {
            if (!Guid.TryParse(User.FindFirstValue("userId"), out var customerId) ||
                !Guid.TryParse(User.FindFirstValue("tenantId"), out var tenantId))
                return Unauthorized(new { message = "Token không hợp lệ" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _orderService.CheckoutAsync(tenantId, customerId, checkoutDto);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            return StatusCode(result.StatusCode, result.Data);
        }

        [HttpPut("{orderId}/cancel")]
        public async Task<IActionResult> CancelMyOrder(Guid orderId)
        {
            if (!Guid.TryParse(User.FindFirstValue("userId"), out var customerId) ||
                !Guid.TryParse(User.FindFirstValue("tenantId"), out var tenantId))
                return Unauthorized(new { message = "Token không hợp lệ" });

            var result = await _orderService.CancelMyOrderAsync(tenantId, customerId, orderId);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            return StatusCode(result.StatusCode, result.Data);
        }
    }
}
