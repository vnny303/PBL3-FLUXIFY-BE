using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace FluxifyAPI.Models
{
    [Table("cart_items")]

    public class CartItem
    {
        [Column("id")]
        public Guid Id { get; set; }
        [Column("cart_id")]
        public Guid CartId { get; set; }
        [Column("product_sku_id")]
        public Guid ProductSkuId { get; set; }
        [Column("quantity")]
        public int Quantity { get; set; }
        [JsonIgnore]
        public Cart Cart { get; set; } = null!;
        [JsonIgnore]
        public ProductSku ProductSku { get; set; } = null!;
    }
}