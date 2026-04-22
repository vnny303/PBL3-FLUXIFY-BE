namespace FluxifyAPI.DTOs.Analytics
{
    public class TenantAnalyticsBreakdownItemDto
    {
        public string Key { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal Revenue { get; set; }
    }
}
