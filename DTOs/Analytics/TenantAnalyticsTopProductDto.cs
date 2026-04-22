namespace FluxifyAPI.DTOs.Analytics
{
    public class TenantAnalyticsTopProductDto
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int QuantitySold { get; set; }
        public decimal Revenue { get; set; }
        public int OrderCount { get; set; }
        public decimal AverageRating { get; set; }
        public int ReviewCount { get; set; }
    }
}
