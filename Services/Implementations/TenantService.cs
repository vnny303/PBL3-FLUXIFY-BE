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

        private static bool HasAnyContentPatch(StorefrontContentConfigDto contentPatch)
        {
            return contentPatch.Home?.HeroImageUrl != null
                || contentPatch.Home?.HeroOverlayOpacity != null
                || contentPatch.Home?.Title != null
                || contentPatch.Home?.Subtitle != null
                || contentPatch.Home?.FeaturedTitle != null
                || contentPatch.Home?.FeaturedSubtitle != null
                || contentPatch.About?.Story != null;
        }

        private static bool HasAnyThemePatch(StorefrontThemeConfigDto themePatch)
        {
            return themePatch.Colors?.Primary != null
                || themePatch.Colors?.Background != null
                || themePatch.Colors?.Text != null
                || themePatch.Typography?.FontFamily != null
                || themePatch.Layout?.BorderRadius != null
                || themePatch.Components?.Header?.Background != null
                || themePatch.Components?.Header?.Text != null
                || themePatch.Components?.Footer?.Background != null
                || themePatch.Components?.Footer?.Text != null
                || themePatch.Components?.ProductCard?.Background != null
                || themePatch.Components?.ProductCard?.Text != null
                || themePatch.Components?.ProductCard?.Price != null
                || themePatch.Components?.ProductCard?.Badge != null;
        }

        private static void ApplyContentPatch(StorefrontContentConfigDto currentContent, StorefrontContentConfigDto contentPatch)
        {
            currentContent.Home ??= new HomeContentConfigDto();
            currentContent.About ??= new AboutContentConfigDto();

            if (contentPatch.Home?.HeroImageUrl != null)
                currentContent.Home.HeroImageUrl = contentPatch.Home.HeroImageUrl;
            if (contentPatch.Home?.HeroOverlayOpacity != null)
                currentContent.Home.HeroOverlayOpacity = contentPatch.Home.HeroOverlayOpacity;
            if (contentPatch.Home?.Title != null)
                currentContent.Home.Title = contentPatch.Home.Title;
            if (contentPatch.Home?.Subtitle != null)
                currentContent.Home.Subtitle = contentPatch.Home.Subtitle;
            if (contentPatch.Home?.FeaturedTitle != null)
                currentContent.Home.FeaturedTitle = contentPatch.Home.FeaturedTitle;
            if (contentPatch.Home?.FeaturedSubtitle != null)
                currentContent.Home.FeaturedSubtitle = contentPatch.Home.FeaturedSubtitle;

            if (contentPatch.About?.Story != null)
                currentContent.About.Story = contentPatch.About.Story;
        }

        private static void ApplyThemePatch(StorefrontThemeConfigDto currentTheme, StorefrontThemeConfigDto themePatch)
        {
            currentTheme.Colors ??= new ThemeColorsConfigDto();
            currentTheme.Typography ??= new ThemeTypographyConfigDto();
            currentTheme.Layout ??= new ThemeLayoutConfigDto();
            currentTheme.Components ??= new ThemeComponentsConfigDto();
            currentTheme.Components.Header ??= new ThemeSurfaceConfigDto();
            currentTheme.Components.Footer ??= new ThemeSurfaceConfigDto();
            currentTheme.Components.ProductCard ??= new ThemeProductCardConfigDto();

            if (themePatch.Colors?.Primary != null)
                currentTheme.Colors.Primary = themePatch.Colors.Primary;
            if (themePatch.Colors?.Background != null)
                currentTheme.Colors.Background = themePatch.Colors.Background;
            if (themePatch.Colors?.Text != null)
                currentTheme.Colors.Text = themePatch.Colors.Text;

            if (themePatch.Typography?.FontFamily != null)
                currentTheme.Typography.FontFamily = themePatch.Typography.FontFamily;

            if (themePatch.Layout?.BorderRadius != null)
                currentTheme.Layout.BorderRadius = themePatch.Layout.BorderRadius;

            if (themePatch.Components?.Header?.Background != null)
                currentTheme.Components.Header.Background = themePatch.Components.Header.Background;
            if (themePatch.Components?.Header?.Text != null)
                currentTheme.Components.Header.Text = themePatch.Components.Header.Text;
            if (themePatch.Components?.Footer?.Background != null)
                currentTheme.Components.Footer.Background = themePatch.Components.Footer.Background;
            if (themePatch.Components?.Footer?.Text != null)
                currentTheme.Components.Footer.Text = themePatch.Components.Footer.Text;
            if (themePatch.Components?.ProductCard?.Background != null)
                currentTheme.Components.ProductCard.Background = themePatch.Components.ProductCard.Background;
            if (themePatch.Components?.ProductCard?.Text != null)
                currentTheme.Components.ProductCard.Text = themePatch.Components.ProductCard.Text;
            if (themePatch.Components?.ProductCard?.Price != null)
                currentTheme.Components.ProductCard.Price = themePatch.Components.ProductCard.Price;
            if (themePatch.Components?.ProductCard?.Badge != null)
                currentTheme.Components.ProductCard.Badge = themePatch.Components.ProductCard.Badge;
        }

        public async Task<ServiceResult<IEnumerable<object>>> GetMyTenantsAsync(Guid ownerId, QueryTenant query)
        {
            query ??= new QueryTenant();
            var tenantQuery = _tenantRepository.GetTenantsByPlatformUser(ownerId);

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
            if (!await _tenantRepository.TenantExists(id))
                return ServiceResult<TenantDto>.Fail(404, "Tenant không tồn tại");

            if (!await _tenantRepository.IsTenantOwner(id, ownerId))
                return ServiceResult<TenantDto>.Forbidden("Bạn không có quyền truy cập tenant này");

            var tenant = await _tenantRepository.GetTenantAsync(id);
            if (tenant == null)
                return ServiceResult<TenantDto>.Fail(404, "Tenant không tồn tại");

            return ServiceResult<TenantDto>.Ok(tenant.ToTenantDto());
        }

        public async Task<ServiceResult<StorefrontTenantLookupDto>> GetTenantBySubdomainAsync(string subdomain)
        {
            var normalizedSubdomain = NormalizeSubdomain(subdomain);
            var tenant = await _tenantRepository.GetTenantBySubdomainAsync(normalizedSubdomain);
            if (tenant == null)
                return ServiceResult<StorefrontTenantLookupDto>.Fail(404, "Tenant không tồn tại");

            // Keep storefront lookup opaque for disabled stores.
            if (tenant.IsActive != true)
                return ServiceResult<StorefrontTenantLookupDto>.Fail(404, "Tenant không tồn tại");

            return ServiceResult<StorefrontTenantLookupDto>.Ok(tenant.ToStorefrontTenantLookupDto());
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
            if (tenantDto.ContainsDeprecatedThemePayload())
                return ServiceResult<object>.Fail(400,
                    "PUT /api/tenants/{id} không hỗ trợ contentConfig/themeConfig. Dùng PATCH /api/tenants/subdomain/{subdomain}/content hoặc /theme.");

            if (!await _tenantRepository.TenantExists(id))
                return ServiceResult<object>.Fail(404, "Tenant không tồn tại");

            if (!await _tenantRepository.IsTenantOwner(id, ownerId))
                return ServiceResult<object>.Forbidden("Bạn không có quyền cập nhật tenant này");

            var tenant = await _tenantRepository.GetTenantAsync(id);
            if (tenant == null)
                return ServiceResult<object>.Fail(404, "Tenant không tồn tại");

            if (!string.IsNullOrWhiteSpace(tenantDto.Subdomain))
            {
                var normalizedSubdomain = NormalizeSubdomain(tenantDto.Subdomain);
                if (!string.Equals(tenant.Subdomain, normalizedSubdomain, StringComparison.OrdinalIgnoreCase)
                    && await _tenantRepository.SubdomainExists(normalizedSubdomain))
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

        public async Task<ServiceResult<object>> UpdateTenantContentAsync(string subdomain, Guid ownerId, StorefrontContentConfigDto contentPatch)
        {
            if (string.IsNullOrWhiteSpace(subdomain))
                return ServiceResult<object>.Fail(400, "Subdomain không hợp lệ");

            if (!HasAnyContentPatch(contentPatch))
                return ServiceResult<object>.Fail(400, "Payload content không có trường hợp lệ để cập nhật");

            var normalizedSubdomain = NormalizeSubdomain(subdomain);
            var tenant = await _tenantRepository.GetTenantBySubdomainAsync(normalizedSubdomain);
            if (tenant == null)
                return ServiceResult<object>.Fail(404, "Tenant không tồn tại");

            if (tenant.OwnerId != ownerId)
                return ServiceResult<object>.Forbidden("Bạn không có quyền cập nhật nội dung tenant này");

            var currentContent = tenant.ContentConfigJson.ToContentConfigDto();
            ApplyContentPatch(currentContent, contentPatch);

            tenant.ContentConfigJson = currentContent.ToContentConfigJson();
            await _tenantRepository.UpdateTenantAsync(tenant);

            return ServiceResult<object>.Ok(new { ContentConfig = currentContent });
        }

        public async Task<ServiceResult<object>> UpdateTenantThemeAsync(string subdomain, Guid ownerId, StorefrontThemeConfigDto themePatch)
        {
            if (string.IsNullOrWhiteSpace(subdomain))
                return ServiceResult<object>.Fail(400, "Subdomain không hợp lệ");

            if (!HasAnyThemePatch(themePatch))
                return ServiceResult<object>.Fail(400, "Payload theme không có trường hợp lệ để cập nhật");

            var normalizedSubdomain = NormalizeSubdomain(subdomain);
            var tenant = await _tenantRepository.GetTenantBySubdomainAsync(normalizedSubdomain);
            if (tenant == null)
                return ServiceResult<object>.Fail(404, "Tenant không tồn tại");

            if (tenant.OwnerId != ownerId)
                return ServiceResult<object>.Forbidden("Bạn không có quyền cập nhật giao diện tenant này");

            var currentTheme = tenant.ThemeConfigJson.ToThemeConfigDto();
            ApplyThemePatch(currentTheme, themePatch);

            tenant.ThemeConfigJson = currentTheme.ToThemeConfigJson();
            await _tenantRepository.UpdateTenantAsync(tenant);

            return ServiceResult<object>.Ok(new { ThemeConfig = currentTheme });
        }

        public async Task<ServiceResult<object>> DeleteTenantAsync(Guid id, Guid ownerId)
        {
            if (!await _tenantRepository.TenantExists(id))
                return ServiceResult<object>.Fail(404, "Tenant không tồn tại");

            if (!await _tenantRepository.IsTenantOwner(id, ownerId))
                return ServiceResult<object>.Forbidden("Bạn không có quyền xóa tenant này");

            await _tenantRepository.DeleteTenantAsync(id);
            return ServiceResult<object>.Ok(new { message = "Xóa tenant thành công" });
        }
    }
}




