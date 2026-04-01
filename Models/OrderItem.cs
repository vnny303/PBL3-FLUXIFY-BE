using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FluxifyAPI.Models
{
    [Table("order_items")]
    public partial class OrderItem
    {
        [Column("id")]
        public Guid Id { get; set; }
        [Column("order_id")]
        public Guid OrderId { get; set; }
        [Column("product_sku_id")]
        public Guid ProductSkuId { get; set; }
        [Column("selected_options")]
        public string? SelectedOptions { get; set; }
        [Column("quantity")]
        public int Quantity { get; set; }
        [Column("unit_price")]
        public decimal UnitPrice { get; set; }
        [JsonIgnore]
        public Order Order { get; set; } = null!;
        [JsonIgnore]
        public ProductSku ProductSku { get; set; } = null!;
    }
}
