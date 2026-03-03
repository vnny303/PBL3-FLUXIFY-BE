using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopifyAPI.Data;
using ShopifyAPI.Models;

namespace ShopifyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TenantsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TenantsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/tenants
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tenant>>> GetTenants()
        {
            return await _context.Tenants
                .Include(t => t.Owner)
                .ToListAsync();
        }

        // GET: api/tenants/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Tenant>> GetTenant(Guid id)
        {
            var tenant = await _context.Tenants
                .Include(t => t.Owner)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tenant == null)
            {
                return NotFound();
            }

            return tenant;
        }

        // GET: api/tenants/subdomain/{subdomain}
        [HttpGet("subdomain/{subdomain}")]
        public async Task<ActionResult<Tenant>> GetTenantBySubdomain(string subdomain)
        {
            var tenant = await _context.Tenants
                .FirstOrDefaultAsync(t => t.Subdomain == subdomain);

            if (tenant == null)
            {
                return NotFound();
            }

            return tenant;
        }

        // POST: api/tenants
        [HttpPost]
        public async Task<ActionResult<Tenant>> CreateTenant(Tenant tenant)
        {
            // Kiểm tra subdomain đã tồn tại chưa
            if (await _context.Tenants.AnyAsync(t => t.Subdomain == tenant.Subdomain))
            {
                return BadRequest("Subdomain already exists");
            }

            tenant.Id = Guid.NewGuid();
            tenant.IsActive = true;

            _context.Tenants.Add(tenant);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTenant), new { id = tenant.Id }, tenant);
        }

        // PUT: api/tenants/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTenant(Guid id, Tenant tenant)
        {
            if (id != tenant.Id)
            {
                return BadRequest();
            }

            _context.Entry(tenant).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Tenants.Any(t => t.Id == id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/tenants/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTenant(Guid id)
        {
            var tenant = await _context.Tenants.FindAsync(id);
            if (tenant == null)
            {
                return NotFound();
            }

            _context.Tenants.Remove(tenant);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}