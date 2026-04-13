using System;
using FluxifyAPI.DTOs.Cart;
using FluxifyAPI.DTOs.Order;

namespace FluxifyAPI.DTOs.Customer
{
    public class CustomerDto
    {
        public Guid Id { get; set; }

        public Guid TenantId { get; set; }

        public string Email { get; set; } = string.Empty;

        public bool? IsActive { get; set; }

        public DateTime? CreatedAt { get; set; }

        public CartDto? Cart { get; set; }
        public List<OrderDto> Orders { get; set; } = new List<OrderDto>();

    }
}

