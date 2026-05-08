using System.ComponentModel.DataAnnotations.Schema;
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
        public double Price { get; set; }

        public int Stock { get; set; }
        [Column("attributes")]
        public string? Attributes { get; set; }

        [Column("img_url")]
        public string imgUrl { get; set; } = string.Empty;
        [JsonIgnore]
        public Product Product { get; set; } = null!;
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public List<Review> Reviews { get; set; } = new List<Review>();
    }
}


