using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FluxifyAPI.Models
{
    [Table("customer_addresses")]
    public class CustomerAddress
    {
        [Column("id")]
        public Guid Id { get; set; }

        [Column("customer_id")]
        public Guid CustomerId { get; set; }

        [Column("tenant_id")]
        public Guid TenantId { get; set; }

        [Column("receiver_name")]
        public string ReceiverName { get; set; } = string.Empty;

        [Column("phone")]
        public string Phone { get; set; } = string.Empty;

        [Column("country")]
        public string Country { get; set; } = string.Empty;

        [Column("province")]
        public string Province { get; set; } = string.Empty;

        [Column("district")]
        public string District { get; set; } = string.Empty;

        [Column("ward")]
        public string Ward { get; set; } = string.Empty;

        [Column("street_address")]
        public string StreetAddress { get; set; } = string.Empty;

        [Column("is_default")]
        public bool IsDefault { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [JsonIgnore]
        public Customer Customer { get; set; } = null!;

        [JsonIgnore]
        public Tenant Tenant { get; set; } = null!;
        
        [JsonIgnore]
        public List<Order> Orders { get; set; } = new List<Order>();
    }
}
