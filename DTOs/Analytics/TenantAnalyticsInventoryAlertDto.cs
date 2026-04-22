namespace FluxifyAPI.DTOs.Analytics
{
    public class TenantAnalyticsInventoryAlertDto
    {
        public Guid ProductSkuId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Stock { get; set; }
        public decimal Price { get; set; }
        public bool IsOutOfStock { get; set; }
    }
}
