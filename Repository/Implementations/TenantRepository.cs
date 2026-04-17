using FluxifyAPI.Data;
using FluxifyAPI.Repository.Interfaces;
using FluxifyAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FluxifyAPI.Repository.Implementations
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
                .Include(t => t.Categories)
                    .ThenInclude(c => c.Products)
                .FirstOrDefaultAsync(t => t.Id == tenantId);
        }

        public async Task<Tenant?> GetTenantBySubdomainAsync(string subdomain)
        {
            return await _context.Tenants
                .FirstOrDefaultAsync(t => t.Subdomain == subdomain);
        }

        public IQueryable<Tenant> GetTenantsByPlatformUser(Guid platformUserId)
        {
            return _context.Tenants
                .Where(t => t.OwnerId == platformUserId)
                .AsNoTracking();
        }

        public async Task<Tenant> CreateTenantAsync(Tenant tenantModel)
        {
            await _context.Tenants.AddAsync(tenantModel);
            await _context.SaveChangesAsync();

            return tenantModel;
        }

        public async Task<Tenant> UpdateTenantAsync(Tenant tenant)
        {
            if (_context.Entry(tenant).State == EntityState.Detached)
                _context.Tenants.Attach(tenant);
            await _context.SaveChangesAsync();
            return tenant;
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

        public async Task<bool> TenantExists(Guid tenantId)
        {
            return await _context.Tenants.AnyAsync(t => t.Id == tenantId);
        }

        public async Task<bool> SubdomainExists(string subdomain)
        {
            return await _context.Tenants.AnyAsync(t => t.Subdomain == subdomain);
        }

        public async Task<bool> IsTenantOwner(Guid tenantId, Guid platformUserId)
        {
            return await _context.Tenants.AnyAsync(t => t.Id == tenantId && t.OwnerId == platformUserId);
        }
    }
}


