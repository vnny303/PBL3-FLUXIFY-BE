using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluxifyAPI.Models;

namespace FluxifyAPI.Interfaces
{
    public interface ITenantRepository
    {
        public Task<Tenant?> GetTenantAsync(Guid tenantId);
        public Task<Tenant?> GetTenantByOwnerAsync(Guid tenantId, Guid ownerId);
        public Task<Tenant?> GetTenantBySubdomainAsync(string subdomain);
        public Task<List<Tenant>> GetTenantsByPlatformUserAsync(Guid platformUserId);
        public Task<Tenant> CreateTenantAsync(Tenant tenantModel);
        public Task<Tenant> UpdateTenantAsync(Tenant tenant);
        public Task<Tenant?> DeleteTenantAsync(Guid tenantId);
        public Task<bool> TenantExists(Guid tenantId);
        public Task<bool> IsTenantOwner(Guid tenantId, Guid platformUserId);
        public Task<bool> SubdomainExists(string subdomain, Guid? excludeTenantId = null);
    }
}