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
    public DbSet<CustomerAddress> CustomerAddresses { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<PlatformUser> PlatformUsers { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductSku> ProductSkus { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<TenantPaymentSetting> TenantPaymentSettings { get; set; }
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

        modelBuilder.Entity<CustomerAddress>()
            .HasIndex(ca => ca.TenantId);

        modelBuilder.Entity<Order>()
            .HasIndex(o => o.TenantId);

        modelBuilder.Entity<Cart>()
            .HasIndex(c => c.TenantId);

        modelBuilder.Entity<Review>()
            .HasIndex(r => r.TenantId);

        modelBuilder.Entity<Review>()
            .HasIndex(r => new { r.TenantId, r.ProductSkuId });

        modelBuilder.Entity<Review>()
            .HasIndex(r => new { r.TenantId, r.CustomerId });

        modelBuilder.Entity<Review>()
            .HasIndex(r => new { r.TenantId, r.ProductSkuId, r.CustomerId })
            .IsUnique();

        modelBuilder.Entity<TenantPaymentSetting>()
            .HasIndex(tps => new { tps.TenantId, tps.IsActive });

        // decimal
        modelBuilder.Entity<OrderItem>()
            .Property(o => o.UnitPrice)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Order>()
            .Property(o => o.TotalAmount)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Order>()
            .Property(o => o.Subtotal)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Order>()
            .Property(o => o.ShippingFee)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Order>()
            .Property(o => o.TaxAmount)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Order>()
            .ToTable(t => t.HasCheckConstraint("CK_orders_shipping_method", "[shipping_method] IS NULL OR [shipping_method] IN ('standard', 'express')"));

        modelBuilder.Entity<ProductSku>()
            .Property(p => p.Price)
            .HasColumnType("decimal(18,2)");

        // default values
        modelBuilder.Entity<ProductSku>()
            .Property(p => p.Stock)
            .HasDefaultValue(0);

        // fix multiple cascade paths
        modelBuilder.Entity<Tenant>()
            .HasOne(t => t.Owner)
            .WithMany(u => u.Tenants)
            .HasForeignKey(t => t.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CustomerAddress>()
            .HasOne(ca => ca.Tenant)
            .WithMany()
            .HasForeignKey(ca => ca.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Order>()
            .HasOne(o => o.Address)
            .WithMany(ca => ca.Orders)
            .HasForeignKey(o => o.AddressId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TenantPaymentSetting>()
            .HasOne(tps => tps.Tenant)
            .WithMany(t => t.PaymentSettings)
            .HasForeignKey(tps => tps.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Customer>()
            .HasOne(c => c.Tenant)
            .WithMany(t => t.Customers)
            .HasForeignKey(c => c.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Category>()
            .HasOne(c => c.Tenant)
            .WithMany(t => t.Categories)
            .HasForeignKey(c => c.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Product>()
            .HasOne(p => p.Tenant)
            .WithMany()
            .HasForeignKey(p => p.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Order>()
            .HasOne(o => o.Tenant)
            .WithMany(t => t.Orders)
            .HasForeignKey(o => o.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Order>()
            .HasOne(o => o.Customer)
            .WithMany(c => c.Orders)
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Cart>()
            .HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(c => c.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Customer>()
            .HasOne(c => c.Cart)
            .WithOne(cart => cart.Customer)
            .HasForeignKey<Cart>(cart => cart.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CartItem>()
            .HasOne(ci => ci.Cart)
            .WithMany(c => c.CartItems)
            .HasForeignKey(ci => ci.CartId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CartItem>()
            .HasOne(ci => ci.ProductSku)
            .WithMany(ps => ps.CartItems)
            .HasForeignKey(ci => ci.ProductSkuId)
            .OnDelete(DeleteBehavior.Restrict);

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

        modelBuilder.Entity<Review>()
            .HasOne(r => r.Tenant)
            .WithMany(t => t.Reviews)
            .HasForeignKey(r => r.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Review>()
            .HasOne(r => r.ProductSku)
            .WithMany(ps => ps.Reviews)
            .HasForeignKey(r => r.ProductSkuId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Review>()
            .HasOne(r => r.Customer)
            .WithMany(c => c.Reviews)
            .HasForeignKey(r => r.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Order)
            .WithMany(o => o.OrderItems)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.ProductSku)
            .WithMany(ps => ps.OrderItems)
            .HasForeignKey(oi => oi.ProductSkuId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}