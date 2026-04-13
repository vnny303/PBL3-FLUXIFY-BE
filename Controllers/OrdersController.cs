using Microsoft.AspNetCore.Mvc;
using FluxifyAPI.DTOs.Order;
using FluxifyAPI.Helpers;
using FluxifyAPI.IServices;

namespace FluxifyAPI.Controllers
{
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
            var result = await _orderService.GetOrdersAsync(tenantId, query);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            return StatusCode(result.StatusCode, result.Data);
        }

        // GET: api/tenants/{tenantId}/orders/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrder(Guid tenantId, Guid id)
        {
            var result = await _orderService.GetOrderAsync(tenantId, id);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            return StatusCode(result.StatusCode, result.Data);
        }

        // POST: api/tenants/{tenantId}/orders
        [HttpPost]
        public async Task<ActionResult<OrderDto>> CreateOrder(Guid tenantId, [FromBody] CreateOrderRequestDto createDto)
        {
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
            var result = await _orderService.DeleteOrderAsync(tenantId, id);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            if (result.StatusCode == 204)
                return NoContent();

            return StatusCode(result.StatusCode, result.Data);
        }
    }
}