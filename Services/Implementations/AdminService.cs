using FluxifyAPI.DTOs.PlatformUser;
using FluxifyAPI.Interfaces;
using FluxifyAPI.Mapper;
using FluxifyAPI.IServices;

namespace FluxifyAPI.Services
{
    public class AdminService : IAdminService
    {
        private readonly IPlatformUserRepository _platformUserRepository;

        public AdminService(IPlatformUserRepository platformUserRepository)
        {
            _platformUserRepository = platformUserRepository;
        }

        public async Task<ServiceResult<IEnumerable<PlatformUserDto>>> GetAllPlatformUsersAsync()
        {
            var users = await _platformUserRepository.GetAllPlatformUsersAsync();
            return ServiceResult<IEnumerable<PlatformUserDto>>.Ok(users.Select(u => u.ToPlatformUserDto()));
        }

        public async Task<ServiceResult<object>> DeletePlatformUserAsync(Guid id)
        {
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
