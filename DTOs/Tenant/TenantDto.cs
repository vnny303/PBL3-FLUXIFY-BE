using System;
using System.Collections.Generic;
using FluxifyAPI.DTOs.Cartegory;
using FluxifyAPI.DTOs.Customer;
using FluxifyAPI.DTOs.Order;

namespace FluxifyAPI.DTOs.Tenant
{
    public class TenantDto
    {
        public Guid Id { get; set; }

        public Guid OwnerId { get; set; }

        public string Subdomain { get; set; } = null!;

        public string StoreName { get; set; } = null!;

        public bool? IsActive { get; set; }

        public List<CategoryDto> Categories { get; set; } = new List<CategoryDto>();

        public List<CustomerDto> Customers { get; set; } = new List<CustomerDto>();

        public List<OrderDto> Orders { get; set; } = new List<OrderDto>();
    }
}

