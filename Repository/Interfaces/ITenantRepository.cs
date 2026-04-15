using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluxifyAPI.Models;

namespace FluxifyAPI.Repository.Interfaces
{
    public interface ITenantRepository
    {
        Task<Tenant?> GetTenantAsync(Guid tenantId);
        Task<Tenant?> GetTenantBySubdomainAsync(string subdomain);
        IQueryable<Tenant> GetTenantsByPlatformUser(Guid platformUserId);
        Task<Tenant> CreateTenantAsync(Tenant tenantModel);
        Task<Tenant> UpdateTenantAsync(Tenant tenant);
        Task<Tenant?> DeleteTenantAsync(Guid tenantId);
        Task<bool> TenantExists(Guid tenantId);
        Task<bool> SubdomainExists(string subdomain);
        Task<bool> IsTenantOwner(Guid tenantId, Guid platformUserId);
    }
}

