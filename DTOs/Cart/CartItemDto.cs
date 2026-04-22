using System;

namespace FluxifyAPI.DTOs.Cart
{
    public class CartItemDto
    {
        public Guid Id { get; set; }

        public Guid CartId { get; set; }

        public Guid ProductSkuId { get; set; }

        public string? ProductName { get; set; }

        public Dictionary<string, string>? SkuAttributes { get; set; }

        public string? SkuDisplayName { get; set; }

        public string? SkuImageUrl { get; set; }

        public decimal? UnitPrice { get; set; }

        public int Quantity { get; set; }
    }
}

