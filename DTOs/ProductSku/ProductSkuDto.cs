using System;

namespace FluxifyAPI.DTOs.ProductSku
{
    public class ProductSkuDto
    {
        public Guid Id { get; set; }

        public Guid ProductId { get; set; }

        public decimal Price { get; set; }

        public int Stock { get; set; }

        public string? Attributes { get; set; }

        public string? imgUrl { get; set; }
    }
}

