using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluxifyAPI.Models;

namespace FluxifyAPI.Repository.Interfaces
{
    public interface IPlatformUserRepository
    {
        Task<List<PlatformUser>> GetAllPlatformUsersAsync();
        Task<PlatformUser?> GetPlatformUserAsync(Guid platformUserId);
        Task<PlatformUser?> GetPlatformUserByEmailAsync(string email);
        Task<PlatformUser?> GetMerchantByEmailAsync(string email);
        Task<bool> PlatformUserEmailExistsAsync(string email);
        Task<PlatformUser> CreatePlatformUserAsync(PlatformUser platformUser);
        Task<PlatformUser> UpdatePlatformUserAsync(PlatformUser platformUser);
        Task<PlatformUser?> DeletePlatformUserAsync(Guid platformUserId);
    }
}

