using FluxifyAPI.DTOs.Tenant;
using FluxifyAPI.Helpers;
using FluxifyAPI.Services.Common;

namespace FluxifyAPI.Services.Interfaces
{
    public interface ITenantService
    {
        Task<ServiceResult<IEnumerable<object>>> GetMyTenantsAsync(Guid ownerId, QueryTenant query);
        Task<ServiceResult<TenantDto>> GetTenantAsync(Guid id, Guid ownerId);
        Task<ServiceResult<TenantDto>> GetTenantBySubdomainAsync(string subdomain);
        Task<ServiceResult<object>> CreateTenantAsync(Guid ownerId, CreateTenantRequestDto tenantDto);
        Task<ServiceResult<object>> UpdateTenantAsync(Guid id, Guid ownerId, UpdateTenantRequestDto tenantDto);
        Task<ServiceResult<object>> DeleteTenantAsync(Guid id, Guid ownerId);
    }
}


