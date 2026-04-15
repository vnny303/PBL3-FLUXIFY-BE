using FluxifyAPI.DTOs.Tenant;
using FluxifyAPI.Helpers;
using FluxifyAPI.Repository.Interfaces;
using FluxifyAPI.Mapper;
using FluxifyAPI.Services.Interfaces;
using FluxifyAPI.Services.Common;
using Microsoft.EntityFrameworkCore;

namespace FluxifyAPI.Services.Implementations
{
    public class TenantService : ITenantService
    {
        private readonly ITenantRepository _tenantRepository;

        public TenantService(ITenantRepository tenantRepository)
        {
            _tenantRepository = tenantRepository;
        }

        private static string NormalizeSubdomain(string subdomain)
        {
            return subdomain.Trim().ToLowerInvariant();
        }
        public async Task<ServiceResult<IEnumerable<object>>> GetMyTenantsAsync(Guid ownerId, QueryTenant query)
        {
            query ??= new QueryTenant();
            var tenantQuery = _tenantRepository.GetTenantsByPlatformUser(ownerId);
            if (!tenantQuery.Any())
                return ServiceResult<IEnumerable<object>>.Ok(Enumerable.Empty<object>());

            var searchTerm = query.SearchTerm;
            if (!string.IsNullOrEmpty(searchTerm))
            {
                if (Guid.TryParse(searchTerm, out var tenantId))
                    tenantQuery = tenantQuery.Where(t => t.StoreName.Contains(searchTerm) || t.Subdomain.Contains(searchTerm) || t.Id == tenantId);
                else
                    tenantQuery = tenantQuery.Where(t => t.StoreName.Contains(searchTerm) || t.Subdomain.Contains(searchTerm));
            }

            if (!string.IsNullOrWhiteSpace(query.StoreName))
            {
                var storeName = query.StoreName.Trim();
                tenantQuery = tenantQuery.Where(t => t.StoreName.Contains(storeName));
            }
            if (!string.IsNullOrWhiteSpace(query.Subdomain))
            {
                var subdomain = query.Subdomain.Trim();
                tenantQuery = tenantQuery.Where(t => t.Subdomain.Contains(subdomain));
            }
            if (query.IsActive.HasValue)
                tenantQuery = tenantQuery.Where(t => t.IsActive == query.IsActive.Value);
            var sortBy = query.SortBy;
            var isDescending = string.Equals(query.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);
            switch (sortBy?.ToLowerInvariant())
            {
                case "storename":
                case "store_name":
                    tenantQuery = isDescending ? tenantQuery.OrderByDescending(t => t.StoreName) : tenantQuery.OrderBy(t => t.StoreName);
                    break;
                case "subdomain":
                    tenantQuery = isDescending ? tenantQuery.OrderByDescending(t => t.Subdomain) : tenantQuery.OrderBy(t => t.Subdomain);
                    break;
                case "isactive":
                case "is_active":
                    tenantQuery = isDescending ? tenantQuery.OrderByDescending(t => t.IsActive) : tenantQuery.OrderBy(t => t.IsActive);
                    break;
                case "id":
                    tenantQuery = isDescending ? tenantQuery.OrderByDescending(t => t.Id) : tenantQuery.OrderBy(t => t.Id);
                    break;
                default:
                    tenantQuery = tenantQuery.OrderBy(t => t.Id);
                    break;
            }

            var skipNumber = (query.Page - 1) * query.PageSize;
            var tenants = await tenantQuery.Skip(skipNumber).Take(query.PageSize).ToListAsync();
            return ServiceResult<IEnumerable<object>>.Ok(tenants.Select(t => t.ToOverallTenantDto()));
        }

        public async Task<ServiceResult<TenantDto>> GetTenantAsync(Guid id, Guid ownerId)
        {
            if (await _tenantRepository.TenantExists(id) || await _tenantRepository.IsTenantOwner(id, ownerId))
                return ServiceResult<TenantDto>.Forbidden("Bạn không có quyền truy cập tenant này");
            var tenant = await _tenantRepository.GetTenantAsync(id);
            return ServiceResult<TenantDto>.Ok(tenant.ToTenantDto());
        }

        public async Task<ServiceResult<TenantDto>> GetTenantBySubdomainAsync(string subdomain)
        {
            if (!await _tenantRepository.SubdomainExists(NormalizeSubdomain(subdomain)))
                return ServiceResult<TenantDto>.Fail(404, "Tenant không tồn tại");
            var tenant = await _tenantRepository.GetTenantBySubdomainAsync(NormalizeSubdomain(subdomain));
            return ServiceResult<TenantDto>.Ok(tenant.ToTenantDto());
        }

        public async Task<ServiceResult<object>> CreateTenantAsync(Guid ownerId, CreateTenantRequestDto tenantDto)
        {
            if (await _tenantRepository.SubdomainExists(NormalizeSubdomain(tenantDto.Subdomain)))
                return ServiceResult<object>.Fail(409, "Subdomain đã tồn tại");
            var tenant = tenantDto.ToTenantFromCreateDto(ownerId);
            await _tenantRepository.CreateTenantAsync(tenant);

            return ServiceResult<object>.Created(tenant.ToOverallTenantDto());
        }

        public async Task<ServiceResult<object>> UpdateTenantAsync(Guid id, Guid ownerId, UpdateTenantRequestDto tenantDto)
        {
            if (await _tenantRepository.TenantExists(id) || await _tenantRepository.IsTenantOwner(id, ownerId))
                return ServiceResult<object>.Forbidden("Bạn không có quyền cập nhật tenant này");
            var tenant = await _tenantRepository.GetTenantAsync(id);
            if (!string.IsNullOrWhiteSpace(tenantDto.Subdomain))
            {
                if (await _tenantRepository.SubdomainExists(NormalizeSubdomain(tenantDto.Subdomain)))
                    return ServiceResult<object>.Fail(409, "Subdomain đã tồn tại");
                tenant.Subdomain = NormalizeSubdomain(tenantDto.Subdomain);
            }
            if (tenantDto.StoreName != null)
                tenant.StoreName = tenantDto.StoreName.Trim();
            if (tenantDto.IsActive.HasValue)
                tenant.IsActive = tenantDto.IsActive;

            var updatedTenant = await _tenantRepository.UpdateTenantAsync(tenant);
            return ServiceResult<object>.Ok(updatedTenant.ToOverallTenantDto());
        }

        public async Task<ServiceResult<object>> DeleteTenantAsync(Guid id, Guid ownerId)
        {
            if (await _tenantRepository.TenantExists(id) || await _tenantRepository.IsTenantOwner(id, ownerId))
                return ServiceResult<object>.Forbidden("Bạn không có quyền xóa tenant này");
            await _tenantRepository.DeleteTenantAsync(id);
            return ServiceResult<object>.Ok(new { message = "Xóa tenant thành công" });
        }
    }
}




