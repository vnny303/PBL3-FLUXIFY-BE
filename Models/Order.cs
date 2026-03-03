using System;
using System.Collections.Generic;

namespace ShopifyAPI.Models;

public partial class Order
{
    public Guid Id { get; set; }

    public Guid TenantId { get; set; }

    public Guid? CustomerId { get; set; }

    public string? Address { get; set; }

    public string? Status { get; set; }

    public string? PaymentMethod { get; set; }

    public string? PaymentStatus { get; set; }

    public decimal TotalAmount { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual Tenant Tenant { get; set; } = null!;
}
