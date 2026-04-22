using FluxifyAPI.DTOs.Analytics;
using FluxifyAPI.Helpers;
using FluxifyAPI.Services.Common;

namespace FluxifyAPI.Services.Interfaces
{
    public interface IAnalyticsService
    {
        Task<ServiceResult<TenantAnalyticsOverviewDto>> GetOverviewAsync(Guid tenantId, Guid platformUserId, QueryTenantAnalytics query);
        Task<ServiceResult<IEnumerable<TenantAnalyticsTopProductDto>>> GetTopProductsAsync(Guid tenantId, Guid platformUserId, QueryTenantAnalytics query);
        Task<ServiceResult<TenantAnalyticsRatingOverviewDto>> GetRatingsAsync(Guid tenantId, Guid platformUserId, QueryTenantAnalytics query);
        Task<ServiceResult<TenantAnalyticsDashboardDto>> GetDashboardAsync(Guid tenantId, Guid platformUserId, QueryTenantAnalytics query);
    }
}
