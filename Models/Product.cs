using System;
using System.Collections.Generic;

namespace ShopifyAPI.Models;

public partial class Product
{
    public Guid Id { get; set; }

    public Guid TenantId { get; set; }

    public Guid? CategoryId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public bool? IsActive { get; set; }

    // Định nghĩa các nhóm thuộc tính của sản phẩm dạng JSON
    // Ví dụ: {"color": ["Đỏ","Xanh","Trắng"], "size": ["S","M","L","XL"]}
    public string? Attributes { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual Category? Category { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual Tenant Tenant { get; set; } = null!;

    public virtual ICollection<ProductSku> ProductSkus { get; set; } = new List<ProductSku>();
}
