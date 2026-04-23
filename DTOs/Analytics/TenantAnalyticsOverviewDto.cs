namespace FluxifyAPI.DTOs.Analytics
{
    public class TenantAnalyticsOverviewDto
    {
        public DateTime FromUtc { get; set; }
        public DateTime ToUtc { get; set; }
        public int TotalOrders { get; set; }
        public int PaidOrders { get; set; }
        public decimal GrossRevenue { get; set; }
        public decimal PaidRevenue { get; set; }
        public decimal AverageOrderValue { get; set; }
        public int NewCustomers { get; set; }
        public int ActiveCustomers { get; set; }
    }
}
