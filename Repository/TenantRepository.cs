using FluxifyAPI.Data;
using FluxifyAPI.Interfaces;
using FluxifyAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FluxifyAPI.Repository
{
    public class TenantRepository : ITenantRepository
    {
        private readonly AppDbContext _context;

        public TenantRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Tenant?> GetTenantAsync(Guid tenantId)
        {
            return await _context.Tenants
                .Include(t => t.Owner)
                .FirstOrDefaultAsync(t => t.Id == tenantId);
        }

        public async Task<Tenant?> GetTenantBySubdomainAsync(string subdomain)
        {
            return await _context.Tenants
                .Include(t => t.Owner)
                .FirstOrDefaultAsync(t => t.Subdomain == subdomain);
        }

        public async Task<List<Tenant>?> GetTenantsByPlatformUserAsync(Guid platformUserId)
        {
            return await _context.Tenants
                .Where(t => t.OwnerId == platformUserId)
                .ToListAsync();
        }

        public async Task<Tenant> CreateTenantAsync(Guid platformUserId, string name, string description)
        {
            var tenant = new Tenant
            {
                Id = Guid.NewGuid(),
                OwnerId = platformUserId,
                StoreName = name,
                Subdomain = description.Trim().ToLowerInvariant(),
                IsActive = true
            };

            await _context.Tenants.AddAsync(tenant);
            await _context.SaveChangesAsync();

            return tenant;
        }

        public async Task<Tenant?> UpdateTenantAsync(Guid tenantId, string name, string description)
        {
            var tenant = await _context.Tenants.FirstOrDefaultAsync(t => t.Id == tenantId);
            if (tenant == null)
            {
                return null;
            }

            tenant.StoreName = name;
            tenant.Subdomain = description.Trim().ToLowerInvariant();

            await _context.SaveChangesAsync();
            return tenant;
        }

        public async Task<Tenant?> DeleteTenantAsync(Guid tenantId)
        {
            var tenant = await _context.Tenants.FirstOrDefaultAsync(t => t.Id == tenantId);
            if (tenant == null)
            {
                return null;
            }

            _context.Tenants.Remove(tenant);
            await _context.SaveChangesAsync();
            return tenant;
        }
    }
}
