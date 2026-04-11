using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluxifyAPI.DTOs.Cartegory;
using FluxifyAPI.DTOs.Customer;
using FluxifyAPI.DTOs.Order;
using FluxifyAPI.DTOs.Tenant;
using FluxifyAPI.DTOs;
using FluxifyAPI.Models;

namespace FluxifyAPI.Mapper
{
    public static class TenantMapper
    {
        public static object ToOverallTenantDto(this Tenant tenant)
        {
            return new
            {
                Id = tenant.Id,
                OwnerId = tenant.OwnerId,
                Subdomain = tenant.Subdomain,
                StoreName = tenant.StoreName,
                IsActive = tenant.IsActive
            };
        }
        public static TenantDto ToTenantDto(this Tenant tenant)
        {
            return new TenantDto
            {
                Id = tenant.Id,
                OwnerId = tenant.OwnerId,
                Subdomain = tenant.Subdomain,
                StoreName = tenant.StoreName,
                IsActive = tenant.IsActive,
                Categories = tenant.Categories?.Select(c => c.ToCategoryDto()).ToList() ?? new List<CategoryDto>(),
                Customers = tenant.Customers?.Select(c => c.ToCustomerDto()).ToList() ?? new List<CustomerDto>(),
                Orders = tenant.Orders?.Select(o => o.ToOrderDto()).ToList() ?? new List<OrderDto>()
            };
        }
        public static Tenant ToTenantFromCreateDto(this CreateTenantRequestDto createDto, Guid ownerId)
        {
            return new Tenant
            {
                Id = Guid.NewGuid(),
                OwnerId = ownerId,
                Subdomain = createDto.Subdomain.Trim().ToLowerInvariant(),
                StoreName = createDto.StoreName.Trim(),
                IsActive = createDto.IsActive ?? true
            };
        }
        public static Tenant ToTenantFromRegisterDto(this RegisterMerchantRequest registerDto, Guid ownerId)
        {
            return new Tenant
            {
                Id = Guid.NewGuid(),
                OwnerId = ownerId,
                Subdomain = registerDto.Subdomain.Trim().ToLowerInvariant(),
                StoreName = registerDto.StoreName.Trim(),
                IsActive = true
            };
        }

        public static Tenant ToTenantFromUpdateDto(this UpdateTenantRequestDto updateDto, Tenant existingTenant)
        {
            if (!string.IsNullOrWhiteSpace(updateDto.Subdomain))
            {
                existingTenant.Subdomain = updateDto.Subdomain.Trim().ToLowerInvariant();
            }
            if (!string.IsNullOrWhiteSpace(updateDto.StoreName))
            {
                existingTenant.StoreName = updateDto.StoreName.Trim();
            }
            if (updateDto.IsActive.HasValue)
            {
                existingTenant.IsActive = updateDto.IsActive;
            }
            return existingTenant;
        }
    }
}