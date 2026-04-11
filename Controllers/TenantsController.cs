using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FluxifyAPI.DTOs.Tenant;
using FluxifyAPI.Mapper;
using FluxifyAPI.Models;
using System.Security.Claims;
using FluxifyAPI.Interfaces;
using FluxifyAPI.Helpers;

namespace FluxifyAPI.Controllers
{
    [Authorize(Roles = "merchant")]
    [Route("api/[controller]")]
    [ApiController]
    public class TenantsController : ControllerBase
    {
        private readonly ITenantRepository _tenantRepository;

        public TenantsController(ITenantRepository tenantRepository)
        {
            _tenantRepository = tenantRepository;
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

            var tenants = await _tenantRepository.GetTenantsByPlatformUserAsync(ownerId, query);

            if (tenants == null || tenants.Count == 0)
                return NotFound(new { message = "Bạn chưa có tenant nào" });
            return Ok(tenants.Select(t => t.ToOverallTenantDto()));
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

            var tenant = await _tenantRepository.GetTenantByOwnerAsync(id, ownerId);
            if (tenant == null)
                return NotFound(new { message = "Bạn không có quyền truy cập cửa hàng này hoặc cửa hàng không tồn tại." });

            return Ok(tenant.ToTenantDto());
        }

        // GET: api/tenants/subdomain/{subdomain}
        [AllowAnonymous]
        [HttpGet("subdomain/{subdomain}")]
        public async Task<ActionResult<TenantDto>> GetTenantBySubdomain([FromRoute] string subdomain)
        {
            var normalizedSubdomain = NormalizeSubdomain(subdomain);

            var tenant = await _tenantRepository.GetTenantBySubdomainAsync(normalizedSubdomain);

            if (tenant == null)
                return NotFound(new { message = "Tenant không tồn tại" });

            return Ok(tenant.ToTenantDto());
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

            var normalizedSubdomain = NormalizeSubdomain(tenantDto.Subdomain);

            if (await _tenantRepository.GetTenantBySubdomainAsync(normalizedSubdomain) != null)
                return Conflict(new { message = "Subdomain đã tồn tại" });
            var tenant = tenantDto.ToTenantFromCreateDto(ownerId);

            await _tenantRepository.CreateTenantAsync(tenant);
            return CreatedAtAction(nameof(GetTenant), new { id = tenant.Id }, tenant.ToOverallTenantDto());
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

            var tenant = await _tenantRepository.GetTenantByOwnerAsync(id, ownerId);
            if (tenant == null)
            {
                if (await _tenantRepository.TenantExists(id))
                    return Forbid();

                return NotFound(new { message = "Tenant không tồn tại" });
            }

            if (!string.IsNullOrWhiteSpace(tenantDto.Subdomain))
            {
                var normalizedSubdomain = NormalizeSubdomain(tenantDto.Subdomain);

                if (await _tenantRepository.SubdomainExists(normalizedSubdomain, id))
                    return Conflict(new { message = "Subdomain đã tồn tại" });

                tenant.Subdomain = normalizedSubdomain;
            }

            if (tenantDto.StoreName != null)
                tenant.StoreName = tenantDto.StoreName.Trim();

            if (tenantDto.IsActive.HasValue)
                tenant.IsActive = tenantDto.IsActive;

            var updatedTenant = await _tenantRepository.UpdateTenantAsync(tenant);

            return Ok(updatedTenant.ToOverallTenantDto());
        }

        // DELETE: api/tenants/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTenant(Guid id)
        {
            var userIdClaim = User.FindFirstValue("userId");
            if (!Guid.TryParse(userIdClaim, out var ownerId))
                return Unauthorized(new { message = "Token không hợp lệ" });
            if (!await _tenantRepository.TenantExists(id))
                return NotFound(new { message = "Tenant không tồn tại" });

            if (!await _tenantRepository.IsTenantOwner(id, ownerId))
                return Forbid();

            var tenant = await _tenantRepository.DeleteTenantAsync(id);

            return Ok(new
            {
                message = "Xóa tenant thành công",
            });
        }
    }
}