using FluxifyAPI.DTOs.Customer;
using FluxifyAPI.DTOs;
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

        public static Customer ToCustomerFromRegisterDto(this RegisterCustomerRequest registerDto, Guid tenantId)
        {
            return new Customer
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                Email = registerDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
        }

        public static Customer ToCustomerFromUpdateDto(this UpdateCustomerRequestDto updateDto, Customer existingCustomer)
        {
            if (!string.IsNullOrWhiteSpace(updateDto.Email))
            {
                existingCustomer.Email = updateDto.Email.Trim();
            }

            if (!string.IsNullOrWhiteSpace(updateDto.Password))
            {
                existingCustomer.PasswordHash = BCrypt.Net.BCrypt.HashPassword(updateDto.Password);
            }

            if (updateDto.IsActive.HasValue)
            {
                existingCustomer.IsActive = updateDto.IsActive;
            }

            return existingCustomer;
        }
    }
}