using FluxifyAPI.Models;

namespace FluxifyAPI.Repository.Interfaces
{
    public interface ITenantPaymentSettingRepository
    {
        Task<List<TenantPaymentSetting>> GetByTenantIdAsync(Guid tenantId);
        Task<TenantPaymentSetting?> GetByIdAsync(Guid id);
        Task<TenantPaymentSetting?> GetActiveByTenantIdAsync(Guid tenantId);
        Task<TenantPaymentSetting> CreateAsync(TenantPaymentSetting setting);
        Task<TenantPaymentSetting> UpdateAsync(TenantPaymentSetting setting);
        Task<bool> DeleteAsync(TenantPaymentSetting setting);
        Task SetOthersInactiveAsync(Guid tenantId, Guid exceptId);
    }
}
