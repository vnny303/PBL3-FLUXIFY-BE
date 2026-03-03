using System;
using System.Collections.Generic;

namespace ShopifyAPI.Models;

public partial class Customer
{
    public Guid Id { get; set; }

    public Guid TenantId { get; set; }

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual Tenant Tenant { get; set; } = null!;
}
