using System.ComponentModel.DataAnnotations;

namespace FluxifyAPI.DTOs.TenantPaymentSetting
{
    public class CreateTenantPaymentSettingDto
    {
        [Required]
        public Guid TenantId { get; set; }

        [Required]
        public string BankName { get; set; } = string.Empty;

        [Required]
        public string BankCode { get; set; } = string.Empty;

        [Required]
        public string BankAccountNumber { get; set; } = string.Empty;

        [Required]
        public string BankAccountName { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }
}
