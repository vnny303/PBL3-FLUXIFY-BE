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
using System.Text.Json;

namespace FluxifyAPI.Mapper
{
    public static class TenantMapper
    {
        private static readonly JsonSerializerOptions TenantConfigSerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        private static T DeserializeOrDefault<T>(string? json) where T : class, new()
        {
            if (string.IsNullOrWhiteSpace(json))
                return new T();

            try
            {
                return JsonSerializer.Deserialize<T>(json, TenantConfigSerializerOptions) ?? new T();
            }
            catch (JsonException)
            {
                return new T();
            }
        }

        private static string SerializeOrDefault<T>(T? value) where T : class, new()
        {
            return JsonSerializer.Serialize(value ?? new T(), TenantConfigSerializerOptions);
        }

        public static StorefrontContentConfigDto ToContentConfigDto(this string? json)
        {
            return DeserializeOrDefault<StorefrontContentConfigDto>(json);
        }

        public static StorefrontThemeConfigDto ToThemeConfigDto(this string? json)
        {
            return DeserializeOrDefault<StorefrontThemeConfigDto>(json);
        }

        public static string ToContentConfigJson(this StorefrontContentConfigDto? config)
        {
            return SerializeOrDefault(config);
        }

        public static string ToThemeConfigJson(this StorefrontThemeConfigDto? config)
        {
            return SerializeOrDefault(config);
        }

        public static object ToOverallTenantDto(this Tenant tenant)
        {
            return new
            {
                Id = tenant.Id,
                OwnerId = tenant.OwnerId,
                Subdomain = tenant.Subdomain,
                StoreName = tenant.StoreName,
                IsActive = tenant.IsActive,
                ContentConfig = tenant.ContentConfigJson.ToContentConfigDto(),
                ThemeConfig = tenant.ThemeConfigJson.ToThemeConfigDto()
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
                ContentConfig = tenant.ContentConfigJson.ToContentConfigDto(),
                ThemeConfig = tenant.ThemeConfigJson.ToThemeConfigDto(),
                Categories = tenant.Categories?.Select(c => c.ToCategoryDto()).ToList() ?? new List<CategoryDto>(),
                Customers = tenant.Customers?.Select(c => c.ToCustomerDto()).ToList() ?? new List<CustomerDto>(),
                Orders = tenant.Orders?.Select(o => o.ToOrderDto()).ToList() ?? new List<OrderDto>()
            };
        }

        public static StorefrontTenantLookupDto ToStorefrontTenantLookupDto(this Tenant tenant)
        {
            return new StorefrontTenantLookupDto
            {
                Id = tenant.Id,
                Subdomain = tenant.Subdomain,
                StoreName = tenant.StoreName,
                IsActive = tenant.IsActive,
                ContentConfig = tenant.ContentConfigJson.ToContentConfigDto(),
                ThemeConfig = tenant.ThemeConfigJson.ToThemeConfigDto()
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
                IsActive = createDto.IsActive ?? true,
                ContentConfigJson = createDto.ContentConfig.ToContentConfigJson(),
                ThemeConfigJson = createDto.ThemeConfig.ToThemeConfigJson()
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
                IsActive = true,
                ContentConfigJson = new StorefrontContentConfigDto().ToContentConfigJson(),
                ThemeConfigJson = new StorefrontThemeConfigDto().ToThemeConfigJson()
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

