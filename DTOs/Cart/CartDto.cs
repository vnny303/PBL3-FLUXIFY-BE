using System;
using System.Collections.Generic;

namespace FluxifyAPI.DTOs.Cart
{
    public class CartDto
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid TenantId { get; set; }

        public List<CartItemDto>? CartItems { get; set; } = new List<CartItemDto>();
    }
}