using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace FluxifyAPI.Models
{
    [Table("customers")]
    public class Customer
    {
        [Column("id")]
        public Guid Id { get; set; }
        [Column("tenant_id")]
        public Guid TenantId { get; set; }
        [Column("email")]
        public string Email { get; set; } = string.Empty;
        [Column("password_hash")]
        public string PasswordHash { get; set; } = string.Empty;
        [Column("is_active")]
        public bool? IsActive { get; set; }
        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }
        public Cart Cart { get; set; } = new Cart();
        public List<Order> Orders { get; set; } = new List<Order>();
        [JsonIgnore]
        public Tenant Tenant { get; set; } = null!;
    }
}
