namespace FluxifyAPI.DTOs.Review
{
    public class ReviewDto
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public Guid ProductSkuId { get; set; }
        public Guid CustomerId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
