using FluxifyAPI.DTOs.CustomerAddress;
using FluxifyAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FluxifyAPI.Controllers
{
    /// <summary>
    /// Customer tự quản lý địa chỉ của mình thông qua JWT (không cần customerId trên URL).
    /// Route: /api/customer/addresses
    /// </summary>
    [Authorize(Roles = "customer")]
    [Route("api/customer/addresses")]
    [ApiController]
    public class CustomerSelfAddressesController : ControllerBase
    {
        private readonly ICustomerAddressService _addressService;

        public CustomerSelfAddressesController(ICustomerAddressService addressService)
        {
            _addressService = addressService;
        }

        private bool TryGetClaims(out Guid customerId, out Guid tenantId)
        {
            customerId = Guid.Empty;
            tenantId = Guid.Empty;
            return Guid.TryParse(User.FindFirstValue("userId"), out customerId)
                && Guid.TryParse(User.FindFirstValue("tenantId"), out tenantId);
        }

        // GET /api/customer/addresses
        [HttpGet]
        public async Task<IActionResult> GetMyAddresses()
        {
            if (!TryGetClaims(out var customerId, out var tenantId))
                return Unauthorized(new { message = "Token không hợp lệ" });

            var addresses = await _addressService.GetAddressesByCustomerIdAsync(tenantId, customerId);
            return Ok(addresses);
        }

        // GET /api/customer/addresses/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMyAddressById(Guid id)
        {
            if (!TryGetClaims(out var customerId, out var tenantId))
                return Unauthorized(new { message = "Token không hợp lệ" });

            var address = await _addressService.GetAddressByIdAsync(tenantId, id);
            if (address == null)
                return NotFound(new { message = "Địa chỉ không tồn tại" });

            if (address.CustomerId != customerId)
                return Forbid();

            return Ok(address);
        }

        // POST /api/customer/addresses
        [HttpPost]
        public async Task<IActionResult> CreateMyAddress([FromBody] CreateCustomerAddressDto dto)
        {
            if (!TryGetClaims(out var customerId, out var tenantId))
                return Unauthorized(new { message = "Token không hợp lệ" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Gán customerId từ JWT, không để client tự truyền
            dto.CustomerId = customerId;

            var address = await _addressService.CreateAddressAsync(tenantId, dto);
            return CreatedAtAction(nameof(GetMyAddressById), new { id = address.Id }, address);
        }

        // PUT /api/customer/addresses/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMyAddress(Guid id, [FromBody] UpdateCustomerAddressDto dto)
        {
            if (!TryGetClaims(out var customerId, out var tenantId))
                return Unauthorized(new { message = "Token không hợp lệ" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await _addressService.GetAddressByIdAsync(tenantId, id);
            if (existing == null)
                return NotFound(new { message = "Địa chỉ không tồn tại" });

            if (existing.CustomerId != customerId)
                return Forbid();

            var updated = await _addressService.UpdateAddressAsync(tenantId, id, dto);
            return Ok(updated);
        }

        // DELETE /api/customer/addresses/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMyAddress(Guid id)
        {
            if (!TryGetClaims(out var customerId, out var tenantId))
                return Unauthorized(new { message = "Token không hợp lệ" });

            var existing = await _addressService.GetAddressByIdAsync(tenantId, id);
            if (existing == null)
                return NotFound(new { message = "Địa chỉ không tồn tại" });

            if (existing.CustomerId != customerId)
                return Forbid();

            await _addressService.DeleteAddressAsync(tenantId, id);
            return NoContent();
        }

        // PATCH /api/customer/addresses/{id}/default
        [HttpPatch("{id}/default")]
        public async Task<IActionResult> SetMyDefaultAddress(Guid id)
        {
            if (!TryGetClaims(out var customerId, out var tenantId))
                return Unauthorized(new { message = "Token không hợp lệ" });

            var result = await _addressService.SetDefaultAddressAsync(tenantId, customerId, id);
            if (!result)
                return NotFound(new { message = "Địa chỉ không tồn tại hoặc không thuộc về bạn" });

            return NoContent();
        }
    }
}