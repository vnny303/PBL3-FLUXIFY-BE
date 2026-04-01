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
                Cart = customer.Cart == null ? null : customer.Cart.ToCartDto(),
                Orders = customer.Orders?.Select(o => o.ToOrderDto()).ToList() ?? new List<OrderDto>()
            };
        }
        public static Customer ToCustomerFromCreateDto(this CreateCustomerRequestDto createDto, Guid tenantId)
        {
            return new Customer
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                Email = createDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(createDto.Password),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}