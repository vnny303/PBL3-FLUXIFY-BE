using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FluxifyAPI.DTOs.Tenant;
using FluxifyAPI.Mapper;
using FluxifyAPI.Models;
using System.Security.Claims;
using FluxifyAPI.Helpers;
using FluxifyAPI.Services.Interfaces;

namespace FluxifyAPI.Controllers
{
    [Authorize(Roles = "merchant")]
    [Route("api/[controller]")]
    [ApiController]
    public class TenantsController : ControllerBase
    {
        private readonly ITenantService _tenantService;

        public TenantsController(ITenantService tenantService)
        {
            _tenantService = tenantService;
        }

        private static string NormalizeSubdomain(string subdomain)
        {
            return subdomain.Trim().ToLowerInvariant();
        }

        // GET: api/tenants/me
        [HttpGet("me")]
        public async Task<IActionResult> GetMyTenants([FromQuery] QueryTenant query)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdClaim = User.FindFirstValue("userId");
            if (!Guid.TryParse(userIdClaim, out var ownerId))
                return Unauthorized(new { message = "Token không hợp lệ hoặc thiếu userId claim" });

            var result = await _tenantService.GetMyTenantsAsync(ownerId, query);
            return Ok(result.Data);
        }
        // // GET: api/tenants
        // [HttpGet]
        // public async Task<IActionResult> GetTenants()
        // {
        //     var tenants = await _tenantRepository.GetTenantsByPlatformUserAsync(Guid.Empty); // Lấy tất cả tenant mà không phân biệt chủ sở hữu

        //     return Ok(tenants.Select(t => t.ToTenantDto()));
        // }

        // GET: api/tenants/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<TenantDto>> GetTenant(Guid id)
        {
            var userIdClaim = User.FindFirstValue("userId");
            if (!Guid.TryParse(userIdClaim, out var ownerId))
                return Unauthorized(new { message = "Token không hợp lệ" });

            var result = await _tenantService.GetTenantAsync(id, ownerId);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            return Ok(result.Data);
        }

        // GET: api/tenants/subdomain/{subdomain}
        [AllowAnonymous]
        [HttpGet("subdomain/{subdomain}")]
        public async Task<ActionResult<TenantDto>> GetTenantBySubdomain([FromRoute] string subdomain)
        {
            var result = await _tenantService.GetTenantBySubdomainAsync(subdomain);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            return Ok(result.Data);
        }

        // POST: api/tenants
        [HttpPost]
        public async Task<ActionResult<TenantDto>> CreateTenant([FromBody] CreateTenantRequestDto tenantDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdClaim = User.FindFirstValue("userId");
            if (!Guid.TryParse(userIdClaim, out var ownerId))
                return Unauthorized(new { message = "Token không hợp lệ" });

            var result = await _tenantService.CreateTenantAsync(ownerId, tenantDto);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            return StatusCode(result.StatusCode, result.Data);
        }

        // PUT: api/tenants/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<TenantDto>> UpdateTenant(Guid id, [FromBody] UpdateTenantRequestDto tenantDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdClaim = User.FindFirstValue("userId");
            if (!Guid.TryParse(userIdClaim, out var ownerId))
                return Unauthorized(new { message = "Token không hợp lệ" });

            var result = await _tenantService.UpdateTenantAsync(id, ownerId, tenantDto);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            return Ok(result.Data);
        }

        // DELETE: api/tenants/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTenant(Guid id)
        {
            var userIdClaim = User.FindFirstValue("userId");
            if (!Guid.TryParse(userIdClaim, out var ownerId))
                return Unauthorized(new { message = "Token không hợp lệ" });

            var result = await _tenantService.DeleteTenantAsync(id, ownerId);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            return Ok(result.Data);
        }
    }
}