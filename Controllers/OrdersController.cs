using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FluxifyAPI.DTOs.Order;
using FluxifyAPI.Helpers;
using FluxifyAPI.Services.Interfaces;

namespace FluxifyAPI.Controllers
{
    [Authorize]
    [Route("api/tenants/{tenantId}/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        public bool IsTenantOwner(Guid tenantId)
        {
            var userTenantId = User.Claims.FirstOrDefault(c => c.Type == "tenantId")?.Value;
            var role = User.Claims.FirstOrDefault(c => c.Type == "role")?.Value;
            if (role != "Owner")
                return false;
            var userId = User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
            // Implement: kiểm tra userId có phải chủ sở hữu tenant này không, có thể thông qua service hoặc database
            return userTenantId != null && Guid.TryParse(userTenantId, out var parsedTenantId) && parsedTenantId == tenantId;
        }
        // GET: api/tenants/{tenantId}/orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders(Guid tenantId, [FromQuery] QueryOrder query)
        {
            if (!IsTenantOwner(tenantId))
                return Unauthorized(new { message = "Bạn không phải là chủ sở hữu của tenant này" });

            var result = await _orderService.GetOrdersAsync(tenantId, query);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            return StatusCode(result.StatusCode, result.Data);
        }

        // GET: api/tenants/{tenantId}/orders/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrder(Guid tenantId, Guid id)
        {
            if (!IsTenantOwner(tenantId))
                return Unauthorized(new { message = "Bạn không phải là chủ sở hữu của tenant này" });

            var result = await _orderService.GetOrderAsync(tenantId, id);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            return StatusCode(result.StatusCode, result.Data);
        }

        // POST: api/tenants/{tenantId}/orders
        [HttpPost]
        public async Task<ActionResult<OrderDto>> CreateOrder(Guid tenantId, [FromBody] CreateOrderRequestDto createDto)
        {
            if (!IsTenantOwner(tenantId))
                return Unauthorized(new { message = "Bạn không phải là chủ sở hữu của tenant này" });
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _orderService.CreateOrderAsync(tenantId, createDto);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            return CreatedAtAction(nameof(GetOrder), new { tenantId, id = result.Data!.Id }, result.Data);
        }

        // PUT: api/tenants/{tenantId}/orders/{id}/status
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(Guid tenantId, Guid id, [FromBody] UpdateOrderStatusRequestDto updateDto)
        {
            if (!IsTenantOwner(tenantId))
                return Unauthorized(new { message = "Bạn không phải là chủ sở hữu của tenant này" });
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _orderService.UpdateOrderStatusAsync(tenantId, id, updateDto);
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
            if (!IsTenantOwner(tenantId))
                return Unauthorized(new { message = "Bạn không phải là chủ sở hữu của tenant này" });

            var result = await _orderService.DeleteOrderAsync(tenantId, id);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            if (result.StatusCode == 204)
                return NoContent();

            return StatusCode(result.StatusCode, result.Data);
        }
        public bool IsCartOwner(Guid tenantId, Guid cartId)
        {
            var userTenantId = User.Claims.FirstOrDefault(c => c.Type == "tenantId")?.Value;
            if (userTenantId == null || !Guid.TryParse(userTenantId, out var parsedTenantId) || parsedTenantId != tenantId)
                return false;
            // Kiểm tra xem cart có thuộc về tenant này không, có thể thông qua service hoặc database
            var userId = User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
            // Implement: kiểm tra userId có phải chủ sở hữu cart này không, có thể thông qua service hoặc database
            // Giả sử bạn có một phương thức trong service để kiểm tra quyền sở hữu cart
            return true; // Thay thế bằng logic thực tế để kiểm tra quyền sở hữu cart
        }
        [HttpGet("orders/customer/{customerId}")]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrdersByCustomerId(Guid customerId)
        {
            // var orders = await _orderService.GetOrdersByCustomerIdAsync(customerId);
            // return Ok(orders);
            return Ok(new { message = "Chức năng này chưa được triển khai" });
        }
        [HttpGet("orders/{orderId}/customer/{customerId}")]
        public async Task<ActionResult<OrderDto>> GetOrderByCustomerId(Guid orderId, Guid customerId)
        {
            // var order = await _orderService.GetOrderByCustomerIdAsync(orderId, customerId);
            // if (order == null)
            //     return NotFound(new { message = "Không tìm thấy đơn hàng cho khách hàng này" });
            // return Ok(order);
            return Ok(new { message = "Chức năng này chưa được triển khai" });
        }
        [HttpPost("customer/{customerId}/checkout")]
        public async Task<ActionResult<OrderDto>> Checkout(Guid customerId)
        {
            // var order = await _orderService.CheckoutAsync(customerId);
            // if (order == null)
            //     return BadRequest(new { message = "Không thể checkout đơn hàng cho khách hàng này" });
            // return Ok(order);
            return Ok(new { message = "Chức năng này chưa được triển khai" });
        }
        [HttpPut("{orderId}/customer/{customerId}/cancel")]
        public async Task<IActionResult> CancelOrder(Guid orderId, Guid customerId)
        {
            // var result = await _orderService.CancelOrderAsync(orderId, customerId);
            // if (!result.Success)
            //     return StatusCode(result.StatusCode, new { message = result.Message });
            // return NoContent();
            return Ok(new { message = "Chức năng này chưa được triển khai" });
        }
    }
}

