using FluxifyAPI.DTOs.TenantPaymentSetting;
using FluxifyAPI.Mapper;
using FluxifyAPI.Models;
using FluxifyAPI.Repository.Interfaces;
using FluxifyAPI.Services.Common;
using FluxifyAPI.Services.Interfaces;

namespace FluxifyAPI.Services.Implementations
{
    public class TenantPaymentSettingService : ITenantPaymentSettingService
    {
        private readonly ITenantPaymentSettingRepository _settingRepository;
        private readonly ITenantRepository _tenantRepository;

        public TenantPaymentSettingService(
            ITenantPaymentSettingRepository settingRepository,
            ITenantRepository tenantRepository)
        {
            _settingRepository = settingRepository;
            _tenantRepository = tenantRepository;
        }

        public async Task<ServiceResult<IEnumerable<TenantPaymentSettingDto>>> GetByTenantIdAsync(Guid tenantId, Guid actorId, bool isAdmin)
        {
            if (!await HasPermissionAsync(tenantId, actorId, isAdmin))
                return ServiceResult<IEnumerable<TenantPaymentSettingDto>>.Forbidden("Bạn không có quyền truy cập tenant này");

            var settings = await _settingRepository.GetByTenantIdAsync(tenantId);
            return ServiceResult<IEnumerable<TenantPaymentSettingDto>>.Ok(settings.Select(x => x.ToDto()));
        }

        public async Task<ServiceResult<TenantPaymentSettingDto>> GetByIdAsync(Guid id, Guid actorId, bool isAdmin)
        {
            var setting = await _settingRepository.GetByIdAsync(id);
            if (setting == null)
                return ServiceResult<TenantPaymentSettingDto>.Fail(404, "Không tìm thấy cấu hình thanh toán");

            if (!await HasPermissionAsync(setting.TenantId, actorId, isAdmin))
                return ServiceResult<TenantPaymentSettingDto>.Forbidden("Bạn không có quyền truy cập tenant này");

            return ServiceResult<TenantPaymentSettingDto>.Ok(setting.ToDto());
        }

        public async Task<ServiceResult<TenantPaymentSettingDto>> CreateAsync(CreateTenantPaymentSettingDto dto, Guid actorId, bool isAdmin)
        {
            if (!await HasPermissionAsync(dto.TenantId, actorId, isAdmin))
                return ServiceResult<TenantPaymentSettingDto>.Forbidden("Bạn không có quyền truy cập tenant này");

            var now = DateTime.UtcNow;
            var setting = new TenantPaymentSetting
            {
                Id = Guid.NewGuid(),
                TenantId = dto.TenantId,
                BankName = dto.BankName.Trim(),
                BankCode = dto.BankCode.Trim(),
                BankAccountNumber = dto.BankAccountNumber.Trim(),
                BankAccountName = dto.BankAccountName.Trim(),
                IsActive = dto.IsActive,
                CreatedAt = now,
                UpdatedAt = now
            };

            var created = await _settingRepository.CreateAsync(setting);
            if (created.IsActive)
                await _settingRepository.SetOthersInactiveAsync(created.TenantId, created.Id);

            return ServiceResult<TenantPaymentSettingDto>.Created(created.ToDto());
        }

        public async Task<ServiceResult<TenantPaymentSettingDto>> UpdateAsync(Guid id, UpdateTenantPaymentSettingDto dto, Guid actorId, bool isAdmin)
        {
            var setting = await _settingRepository.GetByIdAsync(id);
            if (setting == null)
                return ServiceResult<TenantPaymentSettingDto>.Fail(404, "Không tìm thấy cấu hình thanh toán");

            if (!await HasPermissionAsync(setting.TenantId, actorId, isAdmin))
                return ServiceResult<TenantPaymentSettingDto>.Forbidden("Bạn không có quyền truy cập tenant này");

            if (!string.IsNullOrWhiteSpace(dto.BankName))
                setting.BankName = dto.BankName.Trim();
            if (!string.IsNullOrWhiteSpace(dto.BankCode))
                setting.BankCode = dto.BankCode.Trim();
            if (!string.IsNullOrWhiteSpace(dto.BankAccountNumber))
                setting.BankAccountNumber = dto.BankAccountNumber.Trim();
            if (!string.IsNullOrWhiteSpace(dto.BankAccountName))
                setting.BankAccountName = dto.BankAccountName.Trim();
            if (dto.IsActive.HasValue)
                setting.IsActive = dto.IsActive.Value;

            setting.UpdatedAt = DateTime.UtcNow;
            var updated = await _settingRepository.UpdateAsync(setting);
            if (updated.IsActive)
                await _settingRepository.SetOthersInactiveAsync(updated.TenantId, updated.Id);

            return ServiceResult<TenantPaymentSettingDto>.Ok(updated.ToDto());
        }

        public async Task<ServiceResult<object>> DeleteAsync(Guid id, Guid actorId, bool isAdmin)
        {
            var setting = await _settingRepository.GetByIdAsync(id);
            if (setting == null)
                return ServiceResult<object>.Fail(404, "Không tìm thấy cấu hình thanh toán");

            if (!await HasPermissionAsync(setting.TenantId, actorId, isAdmin))
                return ServiceResult<object>.Forbidden("Bạn không có quyền truy cập tenant này");

            await _settingRepository.DeleteAsync(setting);
            return ServiceResult<object>.Ok(new { message = "Xóa cấu hình thanh toán thành công" });
        }

        private async Task<bool> HasPermissionAsync(Guid tenantId, Guid actorId, bool isAdmin)
        {
            if (isAdmin)
                return true;

            return await _tenantRepository.IsTenantOwner(tenantId, actorId);
        }
    }
}
