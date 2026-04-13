using FluxifyAPI.DTOs.PlatformUser;
using FluxifyAPI.Services;

namespace FluxifyAPI.IServices
{
    public interface IAdminService
    {
        Task<ServiceResult<IEnumerable<PlatformUserDto>>> GetAllPlatformUsersAsync();
        Task<ServiceResult<object>> DeletePlatformUserAsync(Guid id);
    }
}
