using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FluxifyAPI.Models
{
    [Table("tenants")]
    public class Tenant
    {
        [Column("id")]
        public Guid Id { get; set; }
        [Column("owner_id")]
        public Guid OwnerId { get; set; }
        [Column("subdomain")]
        public string Subdomain { get; set; } = string.Empty;
        [Column("store_name")]
        public string StoreName { get; set; } = string.Empty;
        [Column("is_active")]
        public bool? IsActive { get; set; }
        public List<Category> Categories { get; set; } = new List<Category>();
        public List<Customer> Customers { get; set; } = new List<Customer>();
        public List<Order> Orders { get; set; } = new List<Order>();
        [JsonIgnore]
        public PlatformUser Owner { get; set; } = null!;
    }
}




