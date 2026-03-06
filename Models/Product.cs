using System;
using System.Collections.Generic;

namespace FluxifyAPI.Models;

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

    public List<CartItem> CartItems { get; set; } = new List<CartItem>();

    public Category? Category { get; set; }

    public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public Tenant Tenant { get; set; } = null!;

    public List<ProductSku> ProductSkus { get; set; } = new List<ProductSku>();
}
