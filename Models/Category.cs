using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FluxifyAPI.Models
{
    [Table("categories")]
    public class Category
    {
        [Column("id")]
        public Guid Id { get; set; }
        [Column("tenant_id")]
        public Guid TenantId { get; set; }
        [Column("name")]
        public string Name { get; set; } = null!;
        [Column("description")]
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true; // ← SỬA: Bỏ dấu ? và set default = true
        // Navigation properties - Thêm attribute để tránh lỗi serialization
        // [JsonIgnore]
        public List<Product>? Products { get; set; } = new List<Product>();
        [JsonIgnore]
        public Tenant Tenant { get; set; } = null!;
    }
}