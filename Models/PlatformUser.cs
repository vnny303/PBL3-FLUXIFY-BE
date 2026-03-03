using System;
using System.Collections.Generic;

namespace ShopifyAPI.Models;

public partial class PlatformUser
{
    public Guid Id { get; set; }

    public string Fullname { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Role { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Tenant> Tenants { get; set; } = new List<Tenant>();
}
