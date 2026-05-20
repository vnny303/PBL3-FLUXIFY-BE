using FluxifyAPI.DTOs.CustomerAddress;
using FluxifyAPI.Repository.Interfaces;
using FluxifyAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FluxifyAPI.Controllers
{
    /// <summary>
    /// Merchant/admin quản lý địa chỉ khách hàng theo tenant rõ ràng.
    /// Customer tự quản lý địa chỉ phải dùng /api/customer/addresses.
    /// </summary>
    [Authorize(Roles = "admin,merchant")]
    [Route("api/tenants/{tenantId}/customers/{customerId}/addresses")]
    [ApiController]
    public class CustomerAddressesController : ControllerBase
    {
        private readonly ICustomerAddressService _addressService;
        private readonly ITenantRepository _tenantRepository;

        public CustomerAddressesController(ICustomerAddressService addressService, ITenantRepository tenantRepository)
        {
            _addressService = addressService;
            _tenantRepository = tenantRepository;
        }

        private async Task<IActionResult?> ValidateTenantAccessAsync(Guid tenantId)
        {
            var role = User.FindFirstValue("role");
            if (string.Equals(role, "admin", StringComparison.OrdinalIgnoreCase))
                return null;

            if (!Guid.TryParse(User.FindFirstValue("userId"), out var platformUserId))
                return Unauthorized(new { message = "Token không hợp lệ hoặc thiếu userId claim" });

            if (!await _tenantRepository.IsTenantOwner(tenantId, platformUserId))
                return Forbid();

            return null;
        }

        [HttpGet]
        public async Task<IActionResult> GetAddresses(Guid tenantId, Guid customerId)
        {
            var accessError = await ValidateTenantAccessAsync(tenantId);
            if (accessError != null) return accessError;

            var addresses = await _addressService.GetAddressesByCustomerIdAsync(tenantId, customerId);
            return Ok(addresses);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAddressById(Guid tenantId, Guid customerId, Guid id)
        {
            var accessError = await ValidateTenantAccessAsync(tenantId);
            if (accessError != null) return accessError;

            var address = await _addressService.GetAddressByIdAsync(tenantId, id);
            if (address == null || address.CustomerId != customerId)
                return NotFound(new { message = "Địa chỉ không tồn tại hoặc không thuộc khách hàng này" });

            return Ok(address);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAddress(Guid tenantId, Guid customerId, [FromBody] CreateCustomerAddressDto dto)
        {
            var accessError = await ValidateTenantAccessAsync(tenantId);
            if (accessError != null) return accessError;

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            dto.CustomerId = customerId;
            var address = await _addressService.CreateAddressAsync(tenantId, dto);
            return CreatedAtAction(nameof(GetAddressById), new { tenantId, customerId, id = address.Id }, address);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAddress(Guid tenantId, Guid customerId, Guid id, [FromBody] UpdateCustomerAddressDto dto)
        {
            var accessError = await ValidateTenantAccessAsync(tenantId);
            if (accessError != null) return accessError;

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await _addressService.GetAddressByIdAsync(tenantId, id);
            if (existing == null || existing.CustomerId != customerId)
                return NotFound(new { message = "Địa chỉ không tồn tại hoặc không thuộc khách hàng này" });

            var address = await _addressService.UpdateAddressAsync(tenantId, id, dto);
            return Ok(address);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAddress(Guid tenantId, Guid customerId, Guid id)
        {
            var accessError = await ValidateTenantAccessAsync(tenantId);
            if (accessError != null) return accessError;

            var existing = await _addressService.GetAddressByIdAsync(tenantId, id);
            if (existing == null || existing.CustomerId != customerId)
                return NotFound(new { message = "Địa chỉ không tồn tại hoặc không thuộc khách hàng này" });

            await _addressService.DeleteAddressAsync(tenantId, id);
            return NoContent();
        }

        [HttpPatch("{id}/default")]
        public async Task<IActionResult> SetDefault(Guid tenantId, Guid customerId, Guid id)
        {
            var accessError = await ValidateTenantAccessAsync(tenantId);
            if (accessError != null) return accessError;

            var result = await _addressService.SetDefaultAddressAsync(tenantId, customerId, id);
            if (!result)
                return NotFound(new { message = "Địa chỉ không tồn tại hoặc không thuộc khách hàng này" });

            return NoContent();
        }
    }
}