using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
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
        [JsonIgnore]
        public string? AttributesJson { get; set; }

        [NotMapped]
        public Dictionary<string, List<string>>? Attributes
        {
            get
            {
                if (string.IsNullOrWhiteSpace(AttributesJson))
                    return null;

                try
                {
                    return JsonSerializer.Deserialize<Dictionary<string, List<string>>>(AttributesJson);
                }
                catch (JsonException)
                {
                    return null;
                }
            }
            set
            {
                AttributesJson = value == null ? null : JsonSerializer.Serialize(value);
            }
        }

        [Column("img_urls")]

        public List<string> imgUrls { get; set; } = new List<string>();

        [Column("detail_sections")]
        public string? DetailSections { get; set; }

        [Column("specifications")]
        public string? Specifications { get; set; }

        [JsonIgnore]
        public Category Category { get; set; } = null!;
        [JsonIgnore]
        public Tenant Tenant { get; set; } = null!;
        public List<ProductSku> ProductSkus { get; set; } = new List<ProductSku>();
    }
}


