using System;

namespace ShopifyAPI.Models;

public partial class ProductSku
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public decimal Price { get; set; }

    public int Stock { get; set; }

    // Tổ hợp thuộc tính cụ thể của SKU này dạng JSON
    // Ví dụ: {"color": "Đỏ", "size": "M"}
    public string? Attributes { get; set; }

    public virtual Product Product { get; set; } = null!;
}
