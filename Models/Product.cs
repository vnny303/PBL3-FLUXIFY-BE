using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FluxifyAPI.Models
{
    [Table("products")]
    public class Product
    {
        [Column("id")]
        public Guid Id { get; set; }
        [Column("tenant_id")]
        public Guid TenantId { get; set; }
        [Column("category_id")]
        public Guid CategoryId { get; set; }
        [Column("name")]
        public string Name { get; set; } = string.Empty;
        [Column("description")]
        public string? Description { get; set; }
        [Column("attributes")]
        // Định nghĩa các nhóm thuộc tính của sản phẩm dạng JSON
        // Ví dụ: {"color": ["Đỏ","Xanh","Trắng"], "size": ["S","M","L","XL"]}
        // public Dictionary<string, List<string>> Attributes { get; set; } = new Dictionary<string, List<string>>();
        public string? Attributes { get; set; }
        [Column("img_urls")]
        public List<string> imgUrls { get; set; } = new List<string>();
        [JsonIgnore]
        public Category Category { get; set; } = null!;
        [JsonIgnore]
        public Tenant Tenant { get; set; } = null!;
        public List<ProductSku> ProductSkus { get; set; } = new List<ProductSku>();
    }
}
