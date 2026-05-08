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
        public Dictionary<string, List<string>>? Attributes { get; set; }
        public List<string>? imgUrls { get; set; }
        public double MinPrice => ProductSkus.Count == 0 ? 0 : ProductSkus.Min(ps => ps.Price);
        public double MaxPrice => ProductSkus.Count == 0 ? 0 : ProductSkus.Max(ps => ps.Price);
        public List<ProductSkuDto> ProductSkus { get; set; } = new List<ProductSkuDto>();
    }
}

