using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FluxifyAPI.DTOs.Tenant;
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
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });
            return StatusCode(result.StatusCode, result.Data);
        }
        // GET: api/tenants/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<TenantDto>> GetTenant(Guid id)
        {
            var userIdClaim = User.FindFirstValue("userId");
            if (!Guid.TryParse(userIdClaim, out var ownerId))
                return Unauthorized(new { message = "Token không hợp lệ hoặc thiếu userId claim" });
            var result = await _tenantService.GetTenantAsync(id, ownerId);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });
            return Ok(result.Data);
        }

        // GET: api/tenants/subdomain/{subdomain}
        [AllowAnonymous]
        [HttpGet("subdomain/{subdomain}")]
        public async Task<ActionResult<StorefrontTenantLookupDto>> GetTenantBySubdomain([FromRoute] string subdomain)
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
                return Unauthorized(new { message = "Token không hợp lệ hoặc thiếu userId claim" });
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
                return Unauthorized(new { message = "Token không hợp lệ hoặc thiếu userId claim" });
            var result = await _tenantService.UpdateTenantAsync(id, ownerId, tenantDto);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });
            return Ok(result.Data);
        }

        // PATCH: api/tenants/subdomain/{subdomain}/content
        [HttpPatch("subdomain/{subdomain}/content")]
        public async Task<IActionResult> UpdateTenantContent([FromRoute] string subdomain, [FromBody] StorefrontContentConfigDto contentPatch)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdClaim = User.FindFirstValue("userId");
            if (!Guid.TryParse(userIdClaim, out var ownerId))
                return Unauthorized(new { message = "Token không hợp lệ hoặc thiếu userId claim" });

            var result = await _tenantService.UpdateTenantContentAsync(subdomain, ownerId, contentPatch);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            return Ok(result.Data);
        }

        // PATCH: api/tenants/subdomain/{subdomain}/theme
        [HttpPatch("subdomain/{subdomain}/theme")]
        public async Task<IActionResult> UpdateTenantTheme([FromRoute] string subdomain, [FromBody] StorefrontThemeConfigDto themePatch)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdClaim = User.FindFirstValue("userId");
            if (!Guid.TryParse(userIdClaim, out var ownerId))
                return Unauthorized(new { message = "Token không hợp lệ hoặc thiếu userId claim" });

            var result = await _tenantService.UpdateTenantThemeAsync(subdomain, ownerId, themePatch);
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

