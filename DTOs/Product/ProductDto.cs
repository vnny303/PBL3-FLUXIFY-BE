using System;
using System.Collections.Generic;
using FluxifyAPI.DTOs.ProductSku;

namespace FluxifyAPI.DTOs.Product
{
    public class ProductDto
    {
        public Guid Id { get; set; }

        public Guid TenantId { get; set; }

        public Guid CategoryId { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public string? Attributes { get; set; }
        public List<string>? imgUrls { get; set; }

        public List<ProductSkuDto> ProductSkus { get; set; } = new List<ProductSkuDto>();
    }
}

