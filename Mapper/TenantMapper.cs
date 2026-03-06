using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluxifyAPI.DTOs.Cartegory;
using FluxifyAPI.DTOs.Customer;
using FluxifyAPI.DTOs.Order;
using FluxifyAPI.DTOs.Tenant;
using FluxifyAPI.Models;

namespace FluxifyAPI.Mapper
{
    public static class TenantMapper
    {
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
    }
}