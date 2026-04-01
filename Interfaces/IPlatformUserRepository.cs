using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluxifyAPI.Models;

namespace FluxifyAPI.Interfaces
{
    public interface IPlatformUserRepository
    {
        public Task<PlatformUser?> GetPlatformUserAsync(Guid tenantId, Guid platformUserId);
        public Task<PlatformUser?> GetPlatformUserByEmailAsync(Guid tenantId, string email);
        public Task<PlatformUser> CreatePlatformUserAsync(Guid tenantId, string name, string email, string password);
        public Task<PlatformUser?> UpdatePlatformUserAsync(Guid tenantId, Guid platformUserId, string name, string email, string password);
        public Task<PlatformUser?> DeletePlatformUserAsync(Guid tenantId, Guid platformUserId);
    }
}