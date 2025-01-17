﻿using Microsoft.EntityFrameworkCore;
namespace Mps.Domain.Entities
{
    public class MpsDbContext(DbContextOptions<MpsDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<ShopOwner> ShopOwners { get; set; }
        public DbSet<Shop> Shops { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<ProductBrand> ProductBrands { get; set; }
        public DbSet<ProductModel> ProductModels { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ProductFeedback> ProductFeedbacks { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<OrderStatus> OrderStatuses { get; set; }
        public DbSet<PaymentStatus> PaymentStatuses { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<PaymentSignature> PaymentSignatures { get; set; }
        public DbSet<UserDevice> UserDevices { get; set; }
        public DbSet<PaymentRef> PaymentRef { get; set; }
        public DbSet<Payout> Payouts { get; set; }
        public DbSet<PayoutStatus> PayoutStatuses { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<ShopSetting> ShopSettings { get; set; }
        public DbSet<UserNotification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasKey(u => u.Id);
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<User>().Property(u => u.Role).IsRequired();
            modelBuilder.Entity<User>().HasIndex(u => u.IdentityId).IsUnique();
            modelBuilder.Entity<User>().Property(u => u.IsActive).IsRequired();
            modelBuilder.Entity<User>().HasOne(u => u.Customer).WithOne().HasForeignKey<Customer>(u => u.Id);
            modelBuilder.Entity<User>().HasOne(u => u.ShopOwner).WithOne().HasForeignKey<ShopOwner>(u => u.UserId);
            modelBuilder.Entity<User>().HasOne(u => u.Staff).WithOne().HasForeignKey<Staff>(u => u.UserId);
            modelBuilder.Entity<User>().HasMany(u => u.UserDevices).WithOne().HasForeignKey(u => u.UserId);

            modelBuilder.Entity<UserDevice>().HasKey(u => u.Id);

            modelBuilder.Entity<Customer>().HasKey(c => c.Id);

            modelBuilder.Entity<ShopOwner>().HasKey(s => s.UserId);
            modelBuilder.Entity<ShopOwner>().HasMany(s => s.Shops).WithOne().HasForeignKey(s => s.ShopOwnerId);

            modelBuilder.Entity<Shop>().HasKey(s => s.Id);
            modelBuilder.Entity<Shop>().Property(s => s.ShopName).IsRequired();
            modelBuilder.Entity<Shop>().HasMany(s => s.Payouts).WithOne().HasForeignKey(s => s.ShopId);
            modelBuilder.Entity<Shop>().Property(s => s.IsAccepted).HasDefaultValue(true);

            modelBuilder.Entity<Staff>().HasKey(s => s.UserId);
            modelBuilder.Entity<Staff>().Property(s => s.StaffCode).HasDefaultValueSql("generate_staff_code()");

            modelBuilder.Entity<Product>().HasKey(s => s.Id);
            modelBuilder.Entity<Product>().HasOne(s => s.Shop).WithMany().HasForeignKey(s => s.ShopId);
            modelBuilder.Entity<Product>().Property(s => s.Name).IsRequired();
            modelBuilder.Entity<Product>().Property(s => s.Price).IsRequired();
            modelBuilder.Entity<Product>().Property(s => s.Stock).IsRequired();
            modelBuilder.Entity<Product>().HasOne(s => s.Category).WithMany().HasForeignKey(s => s.CategoryId);
            modelBuilder.Entity<Product>().HasOne(s => s.Model).WithMany().HasForeignKey(s => s.ModelId);
            modelBuilder.Entity<Product>().HasMany(s => s.Images).WithOne().HasForeignKey(s => s.ProductId);
            modelBuilder.Entity<Product>().HasMany(s => s.Feedbacks).WithOne().HasForeignKey(s => s.ProductId);

            modelBuilder.Entity<ProductCategory>().HasKey(c => c.Id);
            modelBuilder.Entity<ProductCategory>().Property(c => c.Name).IsRequired();
            modelBuilder.Entity<ProductCategory>().HasMany(c => c.Children).WithOne().HasForeignKey(c => c.ParentId);

            modelBuilder.Entity<ProductBrand>().HasKey(b => b.Id);
            modelBuilder.Entity<ProductBrand>().Property(b => b.Name).IsRequired();

            modelBuilder.Entity<ProductModel>().HasKey(m => m.Id);
            modelBuilder.Entity<ProductModel>().Property(m => m.Name).IsRequired();
            modelBuilder.Entity<ProductModel>().HasOne(m => m.Brand).WithMany().HasForeignKey(m => m.BrandId);

            modelBuilder.Entity<ProductImage>().HasKey(i => i.Id);
            modelBuilder.Entity<ProductImage>().Property(i => i.ImagePath).IsRequired();

            modelBuilder.Entity<ProductFeedback>().HasKey(f => f.Id);
            modelBuilder.Entity<ProductFeedback>().HasOne(f => f.Customer).WithMany().HasForeignKey(f => f.UserId);

            modelBuilder.Entity<Order>().HasKey(o => o.Id);
            modelBuilder.Entity<Order>().HasOne(o => o.Customer).WithMany().HasForeignKey(o => o.CustomerId);
            modelBuilder.Entity<Order>().HasOne(o => o.Shop).WithMany().HasForeignKey(o => o.ShopId);
            modelBuilder.Entity<Order>().HasOne(o => o.OrderStatus).WithMany().HasForeignKey(o => o.OrderStatusId);
            modelBuilder.Entity<Order>().HasOne(o => o.PaymentStatus).WithMany().HasForeignKey(o => o.PaymentStatusId);
            modelBuilder.Entity<Order>().HasOne(o => o.PaymentMethod).WithMany().HasForeignKey(o => o.PaymentMethodId);
            modelBuilder.Entity<Order>().HasMany(o => o.OrderDetails).WithOne().HasForeignKey(o => o.OrderId);
            modelBuilder.Entity<Order>().HasMany(o => o.Progresses).WithOne().HasForeignKey(o => o.OrderId);

            modelBuilder.Entity<OrderDetail>().HasKey(o => o.Id);
            modelBuilder.Entity<OrderDetail>().HasOne(o => o.Product).WithMany().HasForeignKey(o => o.ProductId);

            modelBuilder.Entity<OrderProgress>().HasKey(o => o.Id);

            modelBuilder.Entity<OrderStatus>().HasKey(s => s.Id);
            modelBuilder.Entity<OrderStatus>().HasData(
                new OrderStatus { Id = 1, Name = "Pending" },
                new OrderStatus { Id = 2, Name = "Processing" },
                new OrderStatus { Id = 3, Name = "Delivered" },
                new OrderStatus { Id = 4, Name = "Cancelled" },
                new OrderStatus { Id = 5, Name = "Returned" },
                new OrderStatus { Id = 6, Name = "Refunded" },
                new OrderStatus { Id = 7, Name = "Completed" },
                new OrderStatus { Id = 8, Name = "Received" }
                );

            modelBuilder.Entity<PaymentStatus>().HasKey(s => s.Id);
            modelBuilder.Entity<PaymentStatus>().HasData(
                new PaymentStatus { Id = 1, Name = "Pending" },
                new PaymentStatus { Id = 2, Name = "Success" },
                new PaymentStatus { Id = 3, Name = "Failed" },
                new PaymentStatus { Id = 4, Name = "Expired" }
                );

            modelBuilder.Entity<PaymentMethod>().HasKey(m => m.Id);
            modelBuilder.Entity<PaymentMethod>().HasData(
                new PaymentMethod { Id = 1, Name = "VnPay" }
                );

            modelBuilder.Entity<Payment>().HasKey(m => m.Id);
            modelBuilder.Entity<Payment>().HasOne(m => m.PaymentSignature).WithMany().HasForeignKey(m => m.PaymentSignatureId);
            modelBuilder.Entity<Payment>().HasMany(m => m.PaymentRefs).WithOne().HasForeignKey(m => m.PaymentId);

            modelBuilder.Entity<PaymentSignature>().HasKey(m => m.Id);

            modelBuilder.Entity<PaymentRef>().HasKey(m => m.Id);

            modelBuilder.Entity<Payout>().HasKey(p => p.Id);
            modelBuilder.Entity<Payout>().HasOne(p => p.PayoutStatus).WithMany().HasForeignKey(p => p.PayoutStatusId);

            modelBuilder.Entity<PayoutStatus>().HasKey(s => s.Id);
            modelBuilder.Entity<PayoutStatus>().HasData(
                new PayoutStatus { Id = 1, Name = "Pending" },
                new PayoutStatus { Id = 2, Name = "Success" },
                new PayoutStatus { Id = 3, Name = "Failed" }
            );

            modelBuilder.Entity<Setting>().HasKey(s => s.Key);
            modelBuilder.Entity<Setting>().HasData(
                new Setting { Key = "DISCOUNT_0_10", Value = "0" },
                new Setting { Key = "DISCOUNT_10_30", Value = "5" },
                new Setting { Key = "DISCOUNT_30_50", Value = "10" },
                new Setting { Key = "DISCOUNT_50_100", Value = "15" },
                new Setting { Key = "DISCOUNT_100_MAX", Value = "20" }
            );

            modelBuilder.Entity<ShopSetting>().HasKey(s => s.Id);
            modelBuilder.Entity<ShopSetting>().HasOne(s => s.Shop).WithMany().HasForeignKey(s => s.ShopId);

            modelBuilder.Entity<UserNotification>().HasKey(n => n.Id);
            modelBuilder.Entity<UserNotification>().Property(n => n.Title).IsRequired();
            modelBuilder.Entity<UserNotification>().Property(n => n.Body).IsRequired();
            modelBuilder.Entity<UserNotification>().Property(n => n.CreatedAt).IsRequired();
            modelBuilder.Entity<UserNotification>().HasOne(n => n.User).WithMany().HasForeignKey(n => n.UserId);
        }
    }
}
