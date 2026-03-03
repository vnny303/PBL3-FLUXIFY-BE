using System;
using System.Collections.Generic;

namespace ShopifyAPI.Models;

public partial class Tenant
{
    public Guid Id { get; set; }

    public Guid OwnerId { get; set; }

    public string Subdomain { get; set; } = null!;

    public string StoreName { get; set; } = null!;

    public bool? IsActive { get; set; } 

    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();

    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual PlatformUser Owner { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
