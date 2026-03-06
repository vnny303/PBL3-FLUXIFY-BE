using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluxifyAPI.DTOs.Customer;
using FluxifyAPI.DTOs.Order;
using FluxifyAPI.Models;

namespace FluxifyAPI.Mapper
{
    public static class CustomerMapper
    {
        public static CustomerDto ToCustomerDto(this Customer customer)
        {
            return new CustomerDto
            {
                Id = customer.Id,
                TenantId = customer.TenantId,
                Email = customer.Email,
                IsActive = customer.IsActive,
                CreatedAt = customer.CreatedAt,
                Cart = customer.Cart.ToCartDto(),
                Orders = customer.Orders?.Select(o => o.ToOrderDto()).ToList() ?? new List<OrderDto>()
            };
        }
    }
}