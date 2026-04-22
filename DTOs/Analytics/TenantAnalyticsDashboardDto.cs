namespace FluxifyAPI.DTOs.Analytics
{
    public class TenantAnalyticsDashboardDto
    {
        public TenantAnalyticsOverviewDto Overview { get; set; } = new();
        public TenantAnalyticsRatingOverviewDto Ratings { get; set; } = new();
        public List<TenantAnalyticsTopProductDto> TopProducts { get; set; } = new();
    }
}
