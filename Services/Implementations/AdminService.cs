using FluxifyAPI.DTOs.PlatformUser;
using FluxifyAPI.Repository.Interfaces;
using FluxifyAPI.Mapper;
using FluxifyAPI.Services.Interfaces;
using FluxifyAPI.Services.Common;

namespace FluxifyAPI.Services.Implementations
{
    public class AdminService : IAdminService
    {
        private readonly IPlatformUserRepository _platformUserRepository;
        private readonly ITenantRepository _tenantRepository;

        public AdminService(IPlatformUserRepository platformUserRepository, ITenantRepository tenantRepository)
        {
            _platformUserRepository = platformUserRepository;
            _tenantRepository = tenantRepository;
        }

        public async Task<ServiceResult<IEnumerable<PlatformUserDto>>> GetAllPlatformUsersAsync()
        {
            var users = await _platformUserRepository.GetAllPlatformUsersAsync();
            return ServiceResult<IEnumerable<PlatformUserDto>>.Ok(users.Select(u => u.ToPlatformUserDto()));
        }

        public async Task<ServiceResult<object>> DeletePlatformUserAsync(Guid id)
        {
            var user = await _platformUserRepository.GetPlatformUserAsync(id);
            if (user == null)
                return ServiceResult<object>.Fail(404, "Id người dùng không hợp lệ");

            var hasTenants = await _tenantRepository.UserHasTenantsAsync(id);
            if (hasTenants)
                return ServiceResult<object>.Fail(409,
                    "Không thể xóa user vì còn tenant đang hoạt động. Hãy xóa hết tenant của user này trước.");

            var deletedUser = await _platformUserRepository.DeletePlatformUserAsync(id);
            if (deletedUser == null)
                return ServiceResult<object>.Fail(404, "Id người dùng không hợp lệ");

            return ServiceResult<object>.Ok(new
            {
                message = "Xóa người dùng thành công",
                id = deletedUser.Id
            });
        }
    }
}