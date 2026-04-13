using FluxifyAPI.DTOs.Tenant;
using FluxifyAPI.Helpers;
using FluxifyAPI.Repository.Interfaces;
using FluxifyAPI.Mapper;
using FluxifyAPI.Services.Interfaces;
using FluxifyAPI.Services.Common;
using Microsoft.EntityFrameworkCore;

namespace FluxifyAPI.Services.Implementations {
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

            var searchTerm = query.NormalizedSearchTerm;
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

            var sortBy = query.SortBy?.Trim();
            var isDescending = query.NormalizedIsDescending;
            var normalizedSortBy = sortBy?.ToLowerInvariant();

            if (normalizedSortBy == "storename" || normalizedSortBy == "store_name")
                tenantQuery = isDescending ? tenantQuery.OrderByDescending(t => t.StoreName) : tenantQuery.OrderBy(t => t.StoreName);
            else if (normalizedSortBy == "subdomain")
                tenantQuery = isDescending ? tenantQuery.OrderByDescending(t => t.Subdomain) : tenantQuery.OrderBy(t => t.Subdomain);
            else if (normalizedSortBy == "isactive" || normalizedSortBy == "is_active")
                tenantQuery = isDescending ? tenantQuery.OrderByDescending(t => t.IsActive) : tenantQuery.OrderBy(t => t.IsActive);
            else if (normalizedSortBy == "id")
                tenantQuery = isDescending ? tenantQuery.OrderByDescending(t => t.Id) : tenantQuery.OrderBy(t => t.Id);
            else
                tenantQuery = tenantQuery.OrderBy(t => t.Id);

            var pageNumber = query.NormalizedPageNumber;
            var pageSize = query.NormalizedPageSize;
            var skipNumber = (pageNumber - 1) * pageSize;

            var tenants = await tenantQuery.Skip(skipNumber).Take(pageSize).ToListAsync();
            return ServiceResult<IEnumerable<object>>.Ok(tenants.Select(t => t.ToOverallTenantDto()));
        }

        public async Task<ServiceResult<TenantDto>> GetTenantAsync(Guid id, Guid ownerId)
        {
            var tenant = await _tenantRepository.GetTenantByOwnerAsync(id, ownerId);
            if (tenant == null)
                return ServiceResult<TenantDto>.Fail(404, "Bạn không có quyền truy cập cửa hàng này hoặc cửa hàng không tồn tại.");

            return ServiceResult<TenantDto>.Ok(tenant.ToTenantDto());
        }

        public async Task<ServiceResult<TenantDto>> GetTenantBySubdomainAsync(string subdomain)
        {
            var tenant = await _tenantRepository.GetTenantBySubdomainAsync(NormalizeSubdomain(subdomain));
            if (tenant == null)
                return ServiceResult<TenantDto>.Fail(404, "Tenant không tồn tại");

            return ServiceResult<TenantDto>.Ok(tenant.ToTenantDto());
        }

        public async Task<ServiceResult<object>> CreateTenantAsync(Guid ownerId, CreateTenantRequestDto tenantDto)
        {
            var normalizedSubdomain = NormalizeSubdomain(tenantDto.Subdomain);
            if (await _tenantRepository.GetTenantBySubdomainAsync(normalizedSubdomain) != null)
                return ServiceResult<object>.Fail(409, "Subdomain đã tồn tại");

            var tenant = tenantDto.ToTenantFromCreateDto(ownerId);
            await _tenantRepository.CreateTenantAsync(tenant);

            return ServiceResult<object>.Created(tenant.ToOverallTenantDto());
        }

        public async Task<ServiceResult<object>> UpdateTenantAsync(Guid id, Guid ownerId, UpdateTenantRequestDto tenantDto)
        {
            var tenant = await _tenantRepository.GetTenantByOwnerAsync(id, ownerId);
            if (tenant == null)
            {
                if (await _tenantRepository.TenantExists(id))
                    return ServiceResult<object>.Fail(403, "Bạn không có quyền cập nhật tenant này");

                return ServiceResult<object>.Fail(404, "Tenant không tồn tại");
            }

            if (!string.IsNullOrWhiteSpace(tenantDto.Subdomain))
            {
                var normalizedSubdomain = NormalizeSubdomain(tenantDto.Subdomain);
                if (await _tenantRepository.SubdomainExists(normalizedSubdomain, id))
                    return ServiceResult<object>.Fail(409, "Subdomain đã tồn tại");

                tenant.Subdomain = normalizedSubdomain;
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
            if (!await _tenantRepository.TenantExists(id))
                return ServiceResult<object>.Fail(404, "Tenant không tồn tại");

            if (!await _tenantRepository.IsTenantOwner(id, ownerId))
                return ServiceResult<object>.Fail(403, "Bạn không có quyền xóa tenant này");

            await _tenantRepository.DeleteTenantAsync(id);
            return ServiceResult<object>.Ok(new { message = "Xóa tenant thành công" });
        }
    }
}




