using System;

namespace FluxifyAPI.DTOs.Cart
{
    public class CartItemDto
    {
        public Guid Id { get; set; }

        public Guid CartId { get; set; }

        public Guid ProductSkuId { get; set; }

        public int Quantity { get; set; }
    }
}