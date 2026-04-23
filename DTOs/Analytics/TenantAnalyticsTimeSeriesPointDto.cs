namespace FluxifyAPI.DTOs.Analytics
{
    public class TenantAnalyticsTimeSeriesPointDto
    {
        public DateTime PeriodStartUtc { get; set; }
        public string Bucket { get; set; } = "day";
        public int Orders { get; set; }
        public decimal GrossRevenue { get; set; }
        public decimal PaidRevenue { get; set; }
    }
}
