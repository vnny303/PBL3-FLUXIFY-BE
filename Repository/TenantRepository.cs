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

        public async Task<Tenant> CreateTenantAsync(Tenant tenantModel)
        {
            await _context.Tenants.AddAsync(tenantModel);
            await _context.SaveChangesAsync();

            return tenantModel;
        }

        public async Task<Tenant?> UpdateTenantAsync(Guid tenantId, Tenant tenant)
        {
            var existingTenant = await _context.Tenants.FirstOrDefaultAsync(t => t.Id == tenantId);
            if (existingTenant == null)
            {
                return null;
            }

            existingTenant.StoreName = tenant.StoreName;
            existingTenant.Subdomain = tenant.Subdomain;

            await _context.SaveChangesAsync();
            return existingTenant;
        }

        public async Task<Tenant?> DeleteTenantAsync(Guid tenantId)
        {
            var tenantModel = await _context.Tenants.FirstOrDefaultAsync(t => t.Id == tenantId);
            if (tenantModel == null)
                return null;

            _context.Tenants.Remove(tenantModel);
            await _context.SaveChangesAsync();
            return tenantModel;
        }

        public Task<bool> TenantExists(string subdomain)
        {
            return _context.Tenants.AnyAsync(t => t.Subdomain == subdomain);
        }

        public Task<bool> TenantExists(Guid tenantId)
        {
            return _context.Tenants.AnyAsync(t => t.Id == tenantId);
        }

        public Task<bool> IsTenantOwner(Guid tenantId, Guid platformUserId)
        {
            return _context.Tenants.AnyAsync(t => t.Id == tenantId && t.OwnerId == platformUserId);
        }

        public Task<bool> SubdomainExists(string subdomain)
        {
            return _context.Tenants.AnyAsync(t => t.Subdomain == subdomain);
        }
    }
}
