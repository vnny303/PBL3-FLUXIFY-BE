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
        public Task<Tenant?> GetTenantBySubdomainAsync(string subdomain);
        public Task<List<Tenant>?> GetTenantsByPlatformUserAsync(Guid platformUserId);
        public Task<Tenant> CreateTenantAsync(Guid platformUserId, string name, string description);
        public Task<Tenant?> UpdateTenantAsync(Guid tenantId, string name, string description);
        public Task<Tenant?> DeleteTenantAsync(Guid tenantId);
    }
}