namespace FluxifyAPI.DTOs.TenantPaymentSetting
{
    public class UpdateTenantPaymentSettingDto
    {
        public string? BankName { get; set; }
        public string? BankCode { get; set; }
        public string? BankAccountNumber { get; set; }
        public string? BankAccountName { get; set; }
        public bool? IsActive { get; set; }
    }
}
