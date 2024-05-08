﻿using Microsoft.EntityFrameworkCore;
namespace Mps.Domain.Entities
{
    public class MpsDbContext(DbContextOptions<MpsDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasKey(u => u.UserId);
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<User>().Property(u => u.Role).IsRequired();
            modelBuilder.Entity<User>().HasIndex(u => u.IdentityId).IsUnique();

            modelBuilder.Entity<Customer>().HasKey(c => c.UserId);
            modelBuilder.Entity<Customer>().HasOne(c => c.User).WithOne().HasForeignKey<Customer>(c => c.UserId);

            modelBuilder.Entity<Supplier>().HasKey(s => s.UserId);
            modelBuilder.Entity<Supplier>().HasOne(s => s.User).WithOne().HasForeignKey<Supplier>(s => s.UserId);
            modelBuilder.Entity<Supplier>().HasMany(s => s.Shops).WithOne(s => s.Supplier).HasForeignKey(s => s.SupplierId);

            modelBuilder.Entity<Shop>().HasKey(s => s.ShopId);
            modelBuilder.Entity<Shop>().HasOne(s => s.Supplier).WithMany().HasForeignKey(s => s.SupplierId);
            modelBuilder.Entity<Shop>().Property(s => s.ShopName).IsRequired();

            modelBuilder.Entity<Product>().HasKey(s => s.ProductId);
            modelBuilder.Entity<Product>().HasOne(s => s.Shop).WithMany().HasForeignKey(s => s.ShopId);
            modelBuilder.Entity<Product>().Property(s => s.ProductName).IsRequired();
            modelBuilder.Entity<Product>().Property(s => s.Price).IsRequired();
            modelBuilder.Entity<Product>().Property(s => s.Stock).IsRequired();
            modelBuilder.Entity<Product>().HasOne(s => s.Category).WithMany().HasForeignKey(s => s.CategoryId);
            modelBuilder.Entity<Product>().HasOne(s => s.Brand).WithMany().HasForeignKey(s => s.BrandId);
            modelBuilder.Entity<Product>().HasMany(s => s.Images).WithOne().HasForeignKey(s => s.ProductId);

            modelBuilder.Entity<ProductCategory>().HasKey(c => c.CategoryId);
            modelBuilder.Entity<ProductCategory>().Property(c => c.CategoryName).IsRequired();

            modelBuilder.Entity<ProductBrand>().HasKey(b => b.BrandId);
            modelBuilder.Entity<ProductBrand>().Property(b => b.BrandName).IsRequired();

            modelBuilder.Entity<ProductImage>().HasKey(i => i.ProductImageId);
            modelBuilder.Entity<ProductImage>().Property(i => i.ImagePath).IsRequired();

            modelBuilder.Entity<Order>().HasKey(o => o.OrderId);
            modelBuilder.Entity<Order>().HasOne(o => o.Customer).WithMany().HasForeignKey(o => o.CustomerId);
            modelBuilder.Entity<Order>().HasOne(o => o.Shop).WithMany().HasForeignKey(o => o.ShopId);
            modelBuilder.Entity<Order>().HasMany(o => o.OrderDetails).WithOne().HasForeignKey(o => o.OrderId);
            modelBuilder.Entity<Order>().HasOne(o => o.OrderStatus).WithMany().HasForeignKey(o => o.OrderStatusId);
            modelBuilder.Entity<Order>().HasOne(o => o.PaymentStatus).WithMany().HasForeignKey(o => o.PaymentStatusId);
            modelBuilder.Entity<Order>().HasOne(o => o.PaymentMethod).WithMany().HasForeignKey(o => o.PaymentMethodId);
            modelBuilder.Entity<Order>().HasMany(o => o.Progresses).WithOne().HasForeignKey(o => o.OrderId);

            modelBuilder.Entity<OrderDetail>().HasKey(o => o.OrderDetailId);
            modelBuilder.Entity<OrderDetail>().HasOne(o => o.Product).WithMany().HasForeignKey(o => o.ProductId);
            modelBuilder.Entity<OrderDetail>().HasOne(o => o.Order).WithMany().HasForeignKey(o => o.OrderId);

            modelBuilder.Entity<OrderStatus>().HasKey(s => s.OrderStatusId);
            modelBuilder.Entity<OrderStatus>().HasData(
                new OrderStatus { OrderStatusId = 1, OrderStatusName = "Pending" },
                new OrderStatus { OrderStatusId = 2, OrderStatusName = "Processing" },
                new OrderStatus { OrderStatusId = 3, OrderStatusName = "Delivered" },
                new OrderStatus { OrderStatusId = 4, OrderStatusName = "Cancelled" },
                new OrderStatus { OrderStatusId = 5, OrderStatusName = "Returned" },
                new OrderStatus { OrderStatusId = 6, OrderStatusName = "Refunded" },
                new OrderStatus { OrderStatusId = 7, OrderStatusName = "Completed" }
                );

            modelBuilder.Entity<PaymentStatus>().HasKey(s => s.PaymentStatusId);
            modelBuilder.Entity<PaymentStatus>().HasData(
                new PaymentStatus { PaymentStatusId = 1, PaymentStatusName = "Pending" },
                new PaymentStatus { PaymentStatusId = 2, PaymentStatusName = "Processing" },
                new PaymentStatus { PaymentStatusId = 3, PaymentStatusName = "Paid" },
                new PaymentStatus { PaymentStatusId = 4, PaymentStatusName = "Cancelled" },
                new PaymentStatus { PaymentStatusId = 5, PaymentStatusName = "Refunded" }
                );

            modelBuilder.Entity<PaymentMethod>().HasKey(m => m.PaymentMethodId);
            modelBuilder.Entity<PaymentMethod>().HasData(
                new PaymentMethod { PaymentMethodId = 1, PaymentMethodName = "Cash on Delivery" },
                new PaymentMethod { PaymentMethodId = 2, PaymentMethodName = "Credit Card" },
                new PaymentMethod { PaymentMethodId = 3, PaymentMethodName = "Debit Card" },
                new PaymentMethod { PaymentMethodId = 4, PaymentMethodName = "Net Banking" },
                new PaymentMethod { PaymentMethodId = 5, PaymentMethodName = "UPI" }
                );

        }
    }
}
