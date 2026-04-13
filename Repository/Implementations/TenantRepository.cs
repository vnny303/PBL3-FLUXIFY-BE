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

        public async Task<Tenant?> GetTenantByOwnerAsync(Guid tenantId, Guid ownerId)
        {
            return await _context.Tenants
                .Include(t => t.Categories)
                .Include(t => t.Customers)
                .Include(t => t.Orders)
                .FirstOrDefaultAsync(t => t.Id == tenantId && t.OwnerId == ownerId);
        }

        public async Task<Tenant?> GetTenantBySubdomainAsync(string subdomain)
        {
            return await _context.Tenants
                .Include(t => t.Owner)
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

        public Task<bool> TenantExists(Guid tenantId)
        {
            return _context.Tenants.AnyAsync(t => t.Id == tenantId);
        }

        public Task<bool> IsTenantOwner(Guid tenantId, Guid platformUserId)
        {
            return _context.Tenants.AnyAsync(t => t.Id == tenantId && t.OwnerId == platformUserId);
        }

        public Task<bool> SubdomainExists(string subdomain, Guid? excludeTenantId = null)
        {
            return _context.Tenants.AnyAsync(t =>
                t.Subdomain == subdomain &&
                (!excludeTenantId.HasValue || t.Id != excludeTenantId.Value));
        }
    }
}
