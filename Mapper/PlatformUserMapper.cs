using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluxifyAPI.DTOs;
using FluxifyAPI.DTOs.PlatformUser;
using FluxifyAPI.Models;

namespace FluxifyAPI.Mapper
{
    public static class PlatformUserMapper
    {
        public static PlatformUserDto ToPlatformUserDto(this PlatformUser platformUser)
        {
            return new PlatformUserDto
            {
                Id = platformUser.Id,
                Fullname = platformUser.Fullname,
                Email = platformUser.Email,
                Phone = platformUser.Phone,
                Role = platformUser.Role,
                IsActive = platformUser.IsActive,
                CreatedAt = platformUser.CreatedAt,
                Tenants = platformUser.Tenants.Select(t => t.ToTenantDto()).ToList()
            };
        }

        public static PlatformUser ToPlatformUserFromRegisterDto(this RegisterMerchantRequest registerDto)
        {
            return new PlatformUser
            {
                Id = Guid.NewGuid(),
                Fullname = registerDto.FullName,
                Email = registerDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                Role = "merchant",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}