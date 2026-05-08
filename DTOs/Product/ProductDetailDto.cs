using FluxifyAPI.DTOs.ProductSku;

namespace FluxifyAPI.DTOs.Product
{
    public class ProductDetailDto
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public Guid CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Dictionary<string, List<string>>? Attributes { get; set; }
        public List<string>? imgUrls { get; set; }
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public List<DetailSectionDto> DetailSections { get; set; } = new List<DetailSectionDto>();
        public List<SpecificationDto> Specifications { get; set; } = new List<SpecificationDto>();
        public List<ProductSkuDto> ProductSkus { get; set; } = new List<ProductSkuDto>();
    }
}
