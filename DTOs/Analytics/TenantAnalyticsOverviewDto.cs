namespace FluxifyAPI.DTOs.Analytics
{
    public class TenantAnalyticsOverviewDto
    {
        public DateTime FromUtc { get; set; }
        public DateTime ToUtc { get; set; }
        public int TotalOrders { get; set; }
        public int PaidOrders { get; set; }
        public double GrossRevenue { get; set; }
        public double PaidRevenue { get; set; }
        public double AverageOrderValue { get; set; }
        public int NewCustomers { get; set; }
        public int ActiveCustomers { get; set; }
    }
}
