using FluxifyAPI.DTOs.CustomerAddress;
using FluxifyAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FluxifyAPI.Controllers
{
    [Route("api/customers/{customerId}/addresses")]
    [ApiController]
    public class CustomerAddressesController : ControllerBase
    {
        private readonly ICustomerAddressService _addressService;

        public CustomerAddressesController(ICustomerAddressService addressService)
        {
            _addressService = addressService;
        }

        private Guid GetTenantId()
        {
            if (HttpContext.Items["TenantId"] is Guid tenantId)
                return tenantId;
            throw new Exception("TenantId is not available.");
        }

        [HttpGet]
        public async Task<IActionResult> GetAddresses(Guid customerId)
        {
            var tenantId = GetTenantId();
            var addresses = await _addressService.GetAddressesByCustomerIdAsync(tenantId, customerId);
            return Ok(addresses);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAddressById(Guid id)
        {
            var tenantId = GetTenantId();
            var address = await _addressService.GetAddressByIdAsync(tenantId, id);
            if (address == null) return NotFound("Address not found.");
            return Ok(address);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAddress(Guid customerId, [FromBody] CreateCustomerAddressDto dto)
        {
            if (customerId != dto.CustomerId) return BadRequest("Customer ID mismatch.");
            var tenantId = GetTenantId();
            var address = await _addressService.CreateAddressAsync(tenantId, dto);
            return CreatedAtAction(nameof(GetAddressById), new { customerId = customerId, id = address.Id }, address);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAddress(Guid customerId, Guid id, [FromBody] UpdateCustomerAddressDto dto)
        {
            var tenantId = GetTenantId();
            var address = await _addressService.UpdateAddressAsync(tenantId, id, dto);
            if (address == null) return NotFound("Address not found.");
            return Ok(address);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAddress(Guid customerId, Guid id)
        {
            var tenantId = GetTenantId();
            var result = await _addressService.DeleteAddressAsync(tenantId, id);
            if (!result) return NotFound("Address not found.");
            return NoContent();
        }

        [HttpPatch("{id}/default")]
        public async Task<IActionResult> SetDefault(Guid customerId, Guid id)
        {
            var tenantId = GetTenantId();
            var result = await _addressService.SetDefaultAddressAsync(tenantId, customerId, id);
            if (!result) return NotFound("Address not found.");
            return NoContent();
        }
    }
}
