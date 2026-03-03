using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopifyAPI.Models;

[Table("categories")]
public partial class Category
{
    [Column("id")]
    public Guid Id { get; set; }

    [Column("tenant_id")]
    public Guid TenantId { get; set; }

    [Column("name")]
    public string Name { get; set; } = null!;

    [Column("description")]
    public string? Description { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true; // ← SỬA: Bỏ dấu ? và set default = true

    // Navigation properties - Thêm attribute để tránh lỗi serialization
    [System.Text.Json.Serialization.JsonIgnore]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    [System.Text.Json.Serialization.JsonIgnore]
    public virtual Tenant Tenant { get; set; } = null!;
}