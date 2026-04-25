using FluxifyAPI.DTOs.TenantPaymentSetting;
using FluxifyAPI.Services.Common;

namespace FluxifyAPI.Services.Interfaces
{
    public interface ITenantPaymentSettingService
    {
        Task<ServiceResult<IEnumerable<TenantPaymentSettingDto>>> GetByTenantIdAsync(Guid tenantId, Guid actorId, bool isAdmin);
        Task<ServiceResult<TenantPaymentSettingDto>> GetByIdAsync(Guid id, Guid actorId, bool isAdmin);
        Task<ServiceResult<TenantPaymentSettingDto>> CreateAsync(CreateTenantPaymentSettingDto dto, Guid actorId, bool isAdmin);
        Task<ServiceResult<TenantPaymentSettingDto>> UpdateAsync(Guid id, UpdateTenantPaymentSettingDto dto, Guid actorId, bool isAdmin);
        Task<ServiceResult<object>> DeleteAsync(Guid id, Guid actorId, bool isAdmin);
    }
}
