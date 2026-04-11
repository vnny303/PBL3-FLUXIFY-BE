using FluxifyAPI.DTOs.Customer;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using FluxifyAPI.Interfaces;
using FluxifyAPI.Mapper;

namespace FluxifyAPI.Controllers
{
    [Authorize(Roles = "merchant")]
    [Route("api/tenants/{tenantId}/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ITenantRepository _tenantRepository;

        public CustomersController(ICustomerRepository customerRepository, ITenantRepository tenantRepository)
        {
            _customerRepository = customerRepository;
            _tenantRepository = tenantRepository;
        }

        // GET: api/tenants/{tenantId}/customers
        [HttpGet]
        public async Task<IActionResult> GetCustomers(Guid tenantId)
        {
            var userIdClaim = User.FindFirstValue("userId");
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized(new { message = "Token không hợp lệ" });

            var tenant = await _tenantRepository.GetTenantAsync(tenantId);
            if (tenant == null)
                return NotFound(new { message = "Tenant không tồn tại" });

            if (tenant.OwnerId != userId)
                return Forbid();

            var customers = await _customerRepository.GetCustomersByTenantAsync(tenantId);
            return Ok(customers.Select(c => c.ToCustomerDto()));
        }

        // GET: api/tenants/{tenantId}/customers/{customerId}
        [HttpGet("{customerId}")]
        public async Task<IActionResult> GetCustomer(Guid tenantId, Guid customerId)
        {
            var userIdClaim = User.FindFirstValue("userId");
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized(new { message = "Token không hợp lệ" });

            var tenant = await _tenantRepository.GetTenantAsync(tenantId);
            if (tenant == null)
                return NotFound(new { message = "Tenant không tồn tại" });

            if (tenant.OwnerId != userId)
                return Forbid();

            var customer = await _customerRepository.GetCustomerAsync(tenantId, customerId);
            if (customer == null)
                return NotFound(new { message = "Customer không tồn tại" });

            return Ok(customer.ToCustomerDto());
        }
        // GET: api/subdomain/{subdomain}/customers/{customerId} (COI LẠI)
        // GET: api/tenants/{tenantId}/customers/email/{email}
        [HttpGet("email/{email}")]
        public async Task<IActionResult> GetCustomerByEmail(Guid tenantId, string email)
        {
            var userIdClaim = User.FindFirstValue("userId");
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized(new { message = "Token không hợp lệ" });

            var tenant = await _tenantRepository.GetTenantAsync(tenantId);
            if (tenant == null)
                return NotFound(new { message = "Tenant không tồn tại" });

            if (tenant.OwnerId != userId)
                return Forbid();

            var customer = await _customerRepository.GetCustomerByEmailAsync(tenantId, email);
            if (customer == null)
                return NotFound(new { message = "Customer không tồn tại" });

            return Ok(customer.ToCustomerDto());
        }
        // GET: api/tenants/{tenantId}/customers/cart/{cartId}
        [HttpGet("cart/{cartId}")]
        public async Task<IActionResult> GetCustomerByCart(Guid tenantId, Guid cartId)
        {
            var customer = await _customerRepository.GetCustomerByCartAsync(tenantId, cartId);
            if (customer == null)
                return NotFound(new { message = "Customer không tồn tại" });

            return Ok(customer.ToCustomerDto());
        }
        // POST: api/tenants/{tenantId}/customers
        [HttpPost]
        public async Task<IActionResult> CreateCustomer(Guid tenantId, [FromBody] CreateCustomerRequestDto customerDto)
        {
            var tenant = await _tenantRepository.GetTenantAsync(tenantId);
            if (tenant == null)
                return NotFound(new { message = "Tenant không tồn tại" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingCustomer = await _customerRepository.GetCustomerByEmailAsync(tenantId, customerDto.Email);
            if (existingCustomer != null)
                return BadRequest(new { message = "Email đã được đăng ký trong cửa hàng này" });

            var customer = customerDto.ToCustomerFromCreateDto(tenantId);

            var createdCustomer = await _customerRepository.CreateCustomerAsync(customer);

            return CreatedAtAction(
                nameof(GetCustomer),
                new { tenantId, customerId = createdCustomer.Id },
                createdCustomer.ToCustomerDto());
        }
        // PUT: api/tenants/{tenantId}/customers/{customerId}
        [HttpPut("{customerId}")]
        public async Task<IActionResult> UpdateCustomer(Guid tenantId, Guid customerId, [FromBody] UpdateCustomerRequestDto customerDto)
        {
            var userIdClaim = User.FindFirstValue("userId");
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized(new { message = "Token không hợp lệ" });

            var tenant = await _tenantRepository.GetTenantAsync(tenantId);
            if (tenant == null)
                return NotFound(new { message = "Tenant không tồn tại" });

            if (tenant.OwnerId != userId)
                return Forbid();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingCustomer = await _customerRepository.GetCustomerAsync(tenantId, customerId);
            if (existingCustomer == null)
                return NotFound(new { message = "Customer không tồn tại" });

            var updatedCustomer = await _customerRepository.UpdateCustomerAsync(tenantId, customerId, customerDto);
            if (updatedCustomer == null)
                return NotFound(new { message = "Customer không tồn tại" });

            return Ok(updatedCustomer.ToCustomerDto());
        }
        // DELETE: api/tenants/{tenantId}/customers/{customerId}
        [HttpDelete("{customerId}")]
        public async Task<IActionResult> DeleteCustomer(Guid tenantId, Guid customerId)
        {
            var userIdClaim = User.FindFirstValue("userId");
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized(new { message = "Token không hợp lệ" });

            var tenant = await _tenantRepository.GetTenantAsync(tenantId);
            if (tenant == null)
                return NotFound(new { message = "Tenant không tồn tại" });

            if (tenant.OwnerId != userId)
                return Forbid();

            var existingCustomer = await _customerRepository.GetCustomerAsync(tenantId, customerId);
            if (existingCustomer == null)
                return NotFound(new { message = "Customer không tồn tại" });

            await _customerRepository.DeleteCustomerAsync(tenantId, customerId);
            return NoContent();
        }
    }
}