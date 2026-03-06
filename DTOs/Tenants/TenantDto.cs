using System;
using System.Collections.Generic;
using FluxifyAPI.Models;


namespace FluxifyAPI.DTOs.Tenant
{
    public class TenantDto
    {
        public Guid Id { get; set; }

        public Guid OwnerId { get; set; }

        public string Subdomain { get; set; } = null!;

        public string StoreName { get; set; } = null!;

        public bool? IsActive { get; set; }

        public List<Category> Categories { get; set; } = new List<Category>();

        public List<Customer> Customers { get; set; } = new List<Customer>();

        public List<Order> Orders { get; set; } = new List<Order>();

        public PlatformUser Owner { get; set; } = null!;

        public List<Product> Products { get; set; } = new List<Product>();
    }
}