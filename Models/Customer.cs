using System;
using System.Collections.Generic;

namespace FluxifyAPI.Models;

public partial class Customer
{
    public Guid Id { get; set; }

    public Guid TenantId { get; set; }

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public Cart Cart { get; set; }

    public List<Order> Orders { get; set; } = new List<Order>();

    public Tenant Tenant { get; set; } = null!;
}
