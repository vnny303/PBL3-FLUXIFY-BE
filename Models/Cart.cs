using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FluxifyAPI.Models
{
    [Table("carts")]
    public class Cart
    {
        [Column("id")]
        public Guid Id { get; set; }
        [Column("customer_id")]
        public Guid CustomerId { get; set; }
        [Column("tenant_id")]
        public Guid TenantId { get; set; }
        [JsonIgnore]
        public Customer Customer { get; set; } = null!;
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}

