namespace FluxifyAPI.DTOs.TenantPaymentSetting
{
    public class TenantPaymentSettingDto
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public string BankName { get; set; } = string.Empty;
        public string BankCode { get; set; } = string.Empty;
        public string BankAccountNumber { get; set; } = string.Empty;
        public string BankAccountName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
