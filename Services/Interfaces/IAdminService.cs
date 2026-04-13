using FluxifyAPI.DTOs.PlatformUser;
using FluxifyAPI.Services.Common;

namespace FluxifyAPI.Services.Interfaces
{
    public interface IAdminService
    {
        Task<ServiceResult<IEnumerable<PlatformUserDto>>> GetAllPlatformUsersAsync();
        Task<ServiceResult<object>> DeletePlatformUserAsync(Guid id);
    }
}


