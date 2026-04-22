using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FluxifyAPI.Models
{
    [Table("product_skus")]
    public class ProductSku
    {
        [Column("id")]
        public Guid Id { get; set; }
        [Column("product_id")]
        public Guid ProductId { get; set; }
        [Column("price")]
        public decimal Price { get; set; }

        public int Stock { get; set; }
        [Column("attributes")]
        [JsonIgnore]
        public string? AttributesJson { get; set; }

        [NotMapped]
        public Dictionary<string, string>? Attributes
        {
            get
            {
                if (string.IsNullOrWhiteSpace(AttributesJson))
                    return null;

                try
                {
                    return JsonSerializer.Deserialize<Dictionary<string, string>>(AttributesJson);
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

        [Column("img_url")]
        public string imgUrl { get; set; } = string.Empty;
        [JsonIgnore]
        public Product Product { get; set; } = null!;
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}


