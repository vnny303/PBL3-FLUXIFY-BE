using FluxifyAPI.DTOs.Customer;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using FluxifyAPI.Services.Interfaces;

namespace FluxifyAPI.Controllers
{
    [Authorize(Roles = "merchant")]
    [Route("api/tenants/{tenantId}/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        // GET: api/tenants/{tenantId}/customers
        [HttpGet]
        public async Task<IActionResult> GetCustomers(Guid tenantId)
        {
            var userIdClaim = User.FindFirstValue("userId");
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized(new { message = "Token không hợp lệ" });

            var result = await _customerService.GetCustomersAsync(tenantId, userId);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            return StatusCode(result.StatusCode, result.Data);
        }

        // GET: api/tenants/{tenantId}/customers/{customerId}
        [HttpGet("{customerId}")]
        public async Task<IActionResult> GetCustomer(Guid tenantId, Guid customerId)
        {
            var userIdClaim = User.FindFirstValue("userId");
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized(new { message = "Token không hợp lệ" });

            var result = await _customerService.GetCustomerAsync(tenantId, customerId, userId);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            return StatusCode(result.StatusCode, result.Data);
        }
        // GET: api/subdomain/{subdomain}/customers/{customerId} (COI LẠI)
        // GET: api/tenants/{tenantId}/customers/email/{email}
        [HttpGet("email/{email}")]
        public async Task<IActionResult> GetCustomerByEmail(Guid tenantId, string email)
        {
            var userIdClaim = User.FindFirstValue("userId");
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized(new { message = "Token không hợp lệ" });

            var result = await _customerService.GetCustomerByEmailAsync(tenantId, email, userId);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            return StatusCode(result.StatusCode, result.Data);
        }
        // GET: api/tenants/{tenantId}/customers/cart/{cartId}
        [HttpGet("cart/{cartId}")]
        public async Task<IActionResult> GetCustomerByCart(Guid tenantId, Guid cartId)
        {
            var userIdClaim = User.FindFirstValue("userId");
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized(new { message = "Token không hợp lệ" });

            var result = await _customerService.GetCustomerByCartAsync(tenantId, cartId, userId);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            return StatusCode(result.StatusCode, result.Data);
        }
        // POST: api/tenants/{tenantId}/customers
        [HttpPost]
        public async Task<IActionResult> CreateCustomer(Guid tenantId, [FromBody] CreateCustomerRequestDto customerDto)
        {
            var userIdClaim = User.FindFirstValue("userId");
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized(new { message = "Token không hợp lệ" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _customerService.CreateCustomerAsync(tenantId, customerDto, userId);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            return CreatedAtAction(
                nameof(GetCustomer),
                new { tenantId, customerId = result.Data!.Id },
                result.Data);
        }
        // PUT: api/tenants/{tenantId}/customers/{customerId}
        [HttpPut("{customerId}")]
        public async Task<IActionResult> UpdateCustomer(Guid tenantId, Guid customerId, [FromBody] UpdateCustomerRequestDto customerDto)
        {
            var userIdClaim = User.FindFirstValue("userId");
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized(new { message = "Token không hợp lệ" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _customerService.UpdateCustomerAsync(tenantId, customerId, customerDto, userId);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            return StatusCode(result.StatusCode, result.Data);
        }
        // DELETE: api/tenants/{tenantId}/customers/{customerId}
        [HttpDelete("{customerId}")]
        public async Task<IActionResult> DeleteCustomer(Guid tenantId, Guid customerId)
        {
            var userIdClaim = User.FindFirstValue("userId");
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized(new { message = "Token không hợp lệ" });

            var result = await _customerService.DeleteCustomerAsync(tenantId, customerId, userId);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            if (result.StatusCode == 204)
                return NoContent();

            return StatusCode(result.StatusCode, result.Data);
        }
    }
}

