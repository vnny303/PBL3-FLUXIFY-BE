using System.ComponentModel.DataAnnotations.Schema;
namespace FluxifyAPI.Models
{
    [Table("platform_users")]
    public class PlatformUser
    {
        [Column("id")]
        public Guid Id { get; set; }
        [Column("fullname")]
        public string Fullname { get; set; } = string.Empty;
        [Column("email")]
        public string Email { get; set; } = string.Empty;
        [Column("password_hash")]
        public string PasswordHash { get; set; } = string.Empty;
        [Column("phone")]
        public string? Phone { get; set; }
        [Column("role")]
        public string? Role { get; set; }
        [Column("is_active")]
        public bool? IsActive { get; set; }
        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }
        public List<Tenant> Tenants { get; set; } = new List<Tenant>();
    }
}


