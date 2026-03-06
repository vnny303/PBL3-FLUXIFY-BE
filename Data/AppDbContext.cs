using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using FluxifyAPI.Models;

namespace FluxifyAPI.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<PlatformUser> PlatformUsers { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductSku> ProductSkus { get; set; }
    public DbSet<Tenant> Tenants { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // UNIQUE
        modelBuilder.Entity<Customer>()
            .HasIndex(c => new { c.TenantId, c.Email })
            .IsUnique();

        modelBuilder.Entity<Tenant>()
            .HasIndex(t => t.Subdomain)
            .IsUnique();

        modelBuilder.Entity<PlatformUser>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // TENANT INDEX (multi-tenant performance)
        modelBuilder.Entity<Product>()
            .HasIndex(p => p.TenantId);

        modelBuilder.Entity<Category>()
            .HasIndex(c => c.TenantId);

        modelBuilder.Entity<Customer>()
            .HasIndex(c => c.TenantId);

        modelBuilder.Entity<Order>()
            .HasIndex(o => o.TenantId);

        modelBuilder.Entity<Cart>()
            .HasIndex(c => c.TenantId);

        // decimal
        modelBuilder.Entity<OrderItem>()
            .Property(o => o.UnitPrice)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Order>()
            .Property(o => o.TotalAmount)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<ProductSku>()
            .Property(p => p.Price)
            .HasColumnType("decimal(18,2)");

        // default values
        modelBuilder.Entity<ProductSku>()
            .Property(p => p.Stock)
            .HasDefaultValue(0);

        // fix multiple cascade paths
        modelBuilder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ProductSku>()
            .HasOne(ps => ps.Product)
            .WithMany(p => p.ProductSkus)
            .HasForeignKey(ps => ps.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
