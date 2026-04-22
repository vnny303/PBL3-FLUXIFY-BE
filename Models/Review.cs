using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FluxifyAPI.Models
{
    [Table("reviews")]
    public class Review
    {
        [Column("id")]
        public Guid Id { get; set; }

        [Column("tenant_id")]
        public Guid TenantId { get; set; }

        [Column("product_sku_id")]
        public Guid ProductSkuId { get; set; }

        [Column("customer_id")]
        public Guid CustomerId { get; set; }

        [Column("rating")]
        public int Rating { get; set; }

        [Column("comment")]
        public string Comment { get; set; } = string.Empty;

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [JsonIgnore]
        public Tenant Tenant { get; set; } = null!;

        [JsonIgnore]
        public ProductSku ProductSku { get; set; } = null!;

        [JsonIgnore]
        public Customer Customer { get; set; } = null!;
    }
}
