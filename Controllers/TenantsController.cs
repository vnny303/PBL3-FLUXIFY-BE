using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FluxifyAPI.Data;
using FluxifyAPI.DTOs.Tenant;
using FluxifyAPI.Mapper;
using FluxifyAPI.Models;
using System.Security.Claims;

namespace FluxifyAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TenantsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TenantsController(AppDbContext context)
        {
            _context = context;
        }

        private static string NormalizeSubdomain(string subdomain)
        {
            return subdomain.Trim().ToLowerInvariant();
        }

        private async Task<Tenant?> GetTenantDetailsAsync(Guid tenantId)
        {
            return await _context.Tenants
                .Include(t => t.Categories)
                .Include(t => t.Customers)
                .Include(t => t.Orders)
                .FirstOrDefaultAsync(t => t.Id == tenantId);
        }

        // GET: api/tenants/me
        [HttpGet("me")]
        public async Task<IActionResult> GetMyTenants()
        {
            var userIdClaim = User.FindFirstValue("userId");
            if (!Guid.TryParse(userIdClaim, out var ownerId))
                return Unauthorized(new { message = "Token không hợp lệ hoặc thiếu userId claim" });

            var tenants = await _context.Tenants
                .Include(t => t.Categories)
                .Include(t => t.Customers)
                .Include(t => t.Orders)
                .Where(t => t.OwnerId == ownerId)
                .ToListAsync();

            return Ok(tenants.Select(t => t.ToTenantDto()));
        }
        // GET: api/tenants
        [HttpGet]
        public async Task<IActionResult> GetTenants()
        {
            var tenants = await _context.Tenants
                .Include(t => t.Owner)
                .ToListAsync();

            return Ok(tenants.Select(t => t.ToTenantDto()));
        }

        // GET: api/tenants/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<TenantDto>> GetTenant(Guid id)
        {
            var tenant = await GetTenantDetailsAsync(id);

            if (tenant == null)
            {
                return NotFound();
            }

            return Ok(tenant.ToTenantDto());
        }

        // GET: api/tenants/subdomain/{subdomain}
        [HttpGet("subdomain/{subdomain}")]
        public async Task<ActionResult<TenantDto>> GetTenantBySubdomain(string subdomain)
        {
            var normalizedSubdomain = NormalizeSubdomain(subdomain);

            var tenant = await _context.Tenants
                .Include(t => t.Categories)
                .Include(t => t.Customers)
                .Include(t => t.Orders)
                .FirstOrDefaultAsync(t => t.Subdomain == normalizedSubdomain);

            if (tenant == null)
            {
                return NotFound();
            }

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

            if (await _context.Tenants.AnyAsync(t => t.Subdomain == normalizedSubdomain))
                return Conflict(new { message = "Subdomain đã tồn tại" });

            var tenant = new Tenant
            {
                Id = Guid.NewGuid(),
                OwnerId = ownerId,
                Subdomain = normalizedSubdomain,
                StoreName = tenantDto.StoreName.Trim(),
                IsActive = tenantDto.IsActive ?? true
            };

            await _context.Tenants.AddAsync(tenant);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTenant), new { id = tenant.Id }, tenant.ToTenantDto());
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

            var tenant = await _context.Tenants.FirstOrDefaultAsync(t => t.Id == id);
            if (tenant == null)
                return NotFound(new { message = "Tenant không tồn tại" });

            if (tenant.OwnerId != ownerId)
                return Forbid();

            if (!string.IsNullOrWhiteSpace(tenantDto.Subdomain))
            {
                var normalizedSubdomain = NormalizeSubdomain(tenantDto.Subdomain);
                var isDuplicateSubdomain = await _context.Tenants
                    .AnyAsync(t => t.Subdomain == normalizedSubdomain && t.Id != id);

                if (isDuplicateSubdomain)
                    return Conflict(new { message = "Subdomain đã tồn tại" });

                tenant.Subdomain = normalizedSubdomain;
            }

            if (tenantDto.StoreName != null)
                tenant.StoreName = tenantDto.StoreName.Trim();

            if (tenantDto.IsActive.HasValue)
                tenant.IsActive = tenantDto.IsActive;

            await _context.SaveChangesAsync();

            var updatedTenant = await GetTenantDetailsAsync(id);
            if (updatedTenant == null)
                return NotFound(new { message = "Tenant không tồn tại" });

            return Ok(updatedTenant.ToTenantDto());
        }

        // DELETE: api/tenants/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTenant(Guid id)
        {
            var userIdClaim = User.FindFirstValue("userId");
            if (!Guid.TryParse(userIdClaim, out var ownerId))
                return Unauthorized(new { message = "Token không hợp lệ" });

            var tenant = await _context.Tenants.FirstOrDefaultAsync(t => t.Id == id);
            if (tenant == null)
            {
                return NotFound();
            }

            if (tenant.OwnerId != ownerId)
                return Forbid();

            _context.Tenants.Remove(tenant);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}