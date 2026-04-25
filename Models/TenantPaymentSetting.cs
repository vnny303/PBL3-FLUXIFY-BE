using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FluxifyAPI.Models
{
    [Table("tenant_payment_settings")]
    public class TenantPaymentSetting
    {
        [Column("id")]
        public Guid Id { get; set; }

        [Column("tenant_id")]
        public Guid TenantId { get; set; }

        [Column("bank_name")]
        public string BankName { get; set; } = string.Empty;

        [Column("bank_code")]
        public string BankCode { get; set; } = string.Empty;

        [Column("bank_account_number")]
        public string BankAccountNumber { get; set; } = string.Empty;

        [Column("bank_account_name")]
        public string BankAccountName { get; set; } = string.Empty;

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [JsonIgnore]
        public Tenant Tenant { get; set; } = null!;
    }
}
