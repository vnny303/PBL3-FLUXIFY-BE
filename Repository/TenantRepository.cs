using FluxifyAPI.Data;
using FluxifyAPI.Helpers;
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

        public async Task<List<Tenant>> GetTenantsByPlatformUserAsync(Guid platformUserId, QueryTenant query)
        {
            var tenant = _context.Tenants
                .Where(t => t.OwnerId == platformUserId)
                .AsQueryable();

            var searchTerm = query.NormalizedSearchTerm;
            if (!string.IsNullOrEmpty(searchTerm))
                tenant = tenant.Where(t => t.StoreName.Contains(searchTerm) || t.Subdomain.Contains(searchTerm) || t.Id.ToString().Contains(searchTerm));

            if (!string.IsNullOrWhiteSpace(query.StoreName))
            {
                var storeName = query.StoreName.Trim();
                tenant = tenant.Where(t => t.StoreName.Contains(storeName));
            }

            if (!string.IsNullOrWhiteSpace(query.Subdomain))
            {
                var subdomain = query.Subdomain.Trim();
                tenant = tenant.Where(t => t.Subdomain.Contains(subdomain));
            }

            if (query.IsActive.HasValue)
                tenant = tenant.Where(t => t.IsActive == query.IsActive.Value);

            if (!string.IsNullOrEmpty(query.SortBy))
            {
                if (query.SortBy.Equals("storeName", StringComparison.OrdinalIgnoreCase) || query.SortBy.Equals("store_name", StringComparison.OrdinalIgnoreCase))
                    tenant = query.NormalizedIsDescending ? tenant.OrderByDescending(t => t.StoreName) : tenant.OrderBy(t => t.StoreName);
                else if (query.SortBy.Equals("subdomain", StringComparison.OrdinalIgnoreCase))
                    tenant = query.NormalizedIsDescending ? tenant.OrderByDescending(t => t.Subdomain) : tenant.OrderBy(t => t.Subdomain);
                else if (query.SortBy.Equals("isActive", StringComparison.OrdinalIgnoreCase) || query.SortBy.Equals("is_active", StringComparison.OrdinalIgnoreCase))
                    tenant = query.NormalizedIsDescending ? tenant.OrderByDescending(t => t.IsActive) : tenant.OrderBy(t => t.IsActive);
                else if (query.SortBy.Equals("id", StringComparison.OrdinalIgnoreCase))
                    tenant = query.NormalizedIsDescending ? tenant.OrderByDescending(t => t.Id) : tenant.OrderBy(t => t.Id);
            }
            else
                tenant = tenant.OrderBy(t => t.Id); // Mặc định sắp xếp theo Id nếu sortBy không hợp lệ

            var pageNumber = query.NormalizedPageNumber;
            var pageSize = query.NormalizedPageSize;
            var skipNumber = (pageNumber - 1) * pageSize;

            return await tenant.Skip(skipNumber).Take(pageSize).ToListAsync();
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
