using System;
using System.Collections.Generic;
using FluxifyAPI.DTOs.Tenant;

namespace FluxifyAPI.DTOs.PlatformUser
{
    public class PlatformUserDto
    {
        public Guid Id { get; set; }

        public string Fullname { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? Phone { get; set; }

        public string? Role { get; set; }

        public bool? IsActive { get; set; }

        public DateTime? CreatedAt { get; set; }

        public List<TenantDto> Tenants { get; set; } = new List<TenantDto>();
    }
}

