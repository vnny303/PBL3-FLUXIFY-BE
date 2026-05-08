using FluxifyAPI.DTOs.TenantPaymentSetting;
using FluxifyAPI.Models;

namespace FluxifyAPI.Mapper
{
    public static class TenantPaymentSettingMapper
    {
        public static TenantPaymentSettingDto ToDto(this TenantPaymentSetting model)
        {
            return new TenantPaymentSettingDto
            {
                Id = model.Id,
                TenantId = model.TenantId,
                BankName = model.BankName,
                BankCode = model.BankCode,
                BankAccountNumber = model.BankAccountNumber,
                BankAccountName = model.BankAccountName,
                IsActive = model.IsActive,
                CreatedAt = model.CreatedAt,
                UpdatedAt = model.UpdatedAt
            };
        }
    }
}
