﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Mps.Domain.Entities;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Mps.Api.Migrations
{
    [DbContext(typeof(MpsDbContext))]
    [Migration("20240704075437_OrderDetailProductImage")]
    partial class OrderDetailProductImage
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Mps.Domain.Entities.Customer", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("Customers");
                });

            modelBuilder.Entity("Mps.Domain.Entities.Order", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Address")
                        .HasColumnType("text");

                    b.Property<int>("CustomerId")
                        .HasColumnType("integer");

                    b.Property<string>("CustomerName")
                        .HasColumnType("text");

                    b.Property<DateTime?>("DeliveryDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<decimal?>("Discount")
                        .HasColumnType("numeric");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<string>("Note")
                        .HasColumnType("text");

                    b.Property<DateTime>("OrderDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("OrderStatusId")
                        .HasColumnType("integer");

                    b.Property<int>("PaymentMethodId")
                        .HasColumnType("integer");

                    b.Property<int>("PaymentStatusId")
                        .HasColumnType("integer");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("text");

                    b.Property<int>("ShopId")
                        .HasColumnType("integer");

                    b.Property<decimal>("TotalAmount")
                        .HasColumnType("numeric");

                    b.HasKey("Id");

                    b.HasIndex("CustomerId");

                    b.HasIndex("OrderStatusId");

                    b.HasIndex("PaymentMethodId");

                    b.HasIndex("PaymentStatusId");

                    b.HasIndex("ShopId");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("Mps.Domain.Entities.OrderDetail", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<decimal>("Discount")
                        .HasColumnType("numeric");

                    b.Property<int>("OrderId")
                        .HasColumnType("integer");

                    b.Property<decimal>("Price")
                        .HasColumnType("numeric");

                    b.Property<int>("ProductId")
                        .HasColumnType("integer");

                    b.Property<string>("ProductImage")
                        .HasColumnType("text");

                    b.Property<string>("ProductName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Quantity")
                        .HasColumnType("integer");

                    b.Property<decimal>("Total")
                        .HasColumnType("numeric");

                    b.HasKey("Id");

                    b.HasIndex("OrderId");

                    b.HasIndex("ProductId");

                    b.ToTable("OrderDetails");
                });

            modelBuilder.Entity("Mps.Domain.Entities.OrderProgress", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("OrderId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("OrderId");

                    b.ToTable("OrderProgress");
                });

            modelBuilder.Entity("Mps.Domain.Entities.OrderStatus", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("OrderStatuses");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Pending"
                        },
                        new
                        {
                            Id = 2,
                            Name = "Processing"
                        },
                        new
                        {
                            Id = 3,
                            Name = "Delivered"
                        },
                        new
                        {
                            Id = 4,
                            Name = "Cancelled"
                        },
                        new
                        {
                            Id = 5,
                            Name = "Returned"
                        },
                        new
                        {
                            Id = 6,
                            Name = "Refunded"
                        },
                        new
                        {
                            Id = 7,
                            Name = "Completed"
                        });
                });

            modelBuilder.Entity("Mps.Domain.Entities.Payment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Content")
                        .HasColumnType("text");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int?>("CreatedBy")
                        .HasColumnType("integer");

                    b.Property<string>("Currency")
                        .HasColumnType("text");

                    b.Property<DateTime?>("ExpireDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Language")
                        .HasColumnType("text");

                    b.Property<DateTime?>("PaymentDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("PaymentDestinationId")
                        .HasColumnType("text");

                    b.Property<int?>("PaymentSignatureId")
                        .HasColumnType("integer");

                    b.Property<int>("PaymentStatusId")
                        .HasColumnType("integer");

                    b.Property<decimal>("RequiredAmount")
                        .HasColumnType("numeric");

                    b.HasKey("Id");

                    b.HasIndex("PaymentSignatureId");

                    b.HasIndex("PaymentStatusId");

                    b.ToTable("Payments");
                });

            modelBuilder.Entity("Mps.Domain.Entities.PaymentMethod", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("PaymentMethods");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "VnPay"
                        });
                });

            modelBuilder.Entity("Mps.Domain.Entities.PaymentRef", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("MerchantId")
                        .HasColumnType("integer");

                    b.Property<int>("PaymentId")
                        .HasColumnType("integer");

                    b.Property<int?>("RefId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("PaymentId");

                    b.ToTable("PaymentRef");
                });

            modelBuilder.Entity("Mps.Domain.Entities.PaymentSignature", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<bool>("IsValid")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("SignDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int?>("SignOwn")
                        .HasColumnType("integer");

                    b.Property<string>("SignValue")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("PaymentSignatures");
                });

            modelBuilder.Entity("Mps.Domain.Entities.PaymentStatus", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("PaymentStatuses");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Pending"
                        },
                        new
                        {
                            Id = 2,
                            Name = "Success"
                        },
                        new
                        {
                            Id = 3,
                            Name = "Failed"
                        },
                        new
                        {
                            Id = 4,
                            Name = "Expired"
                        });
                });

            modelBuilder.Entity("Mps.Domain.Entities.Payout", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<decimal?>("Amount")
                        .HasColumnType("numeric");

                    b.Property<string>("BatchId")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Currency")
                        .HasColumnType("text");

                    b.Property<decimal?>("ExpectAmount")
                        .HasColumnType("numeric");

                    b.Property<DateTime>("MonthToDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("PayoutStatusId")
                        .HasColumnType("integer");

                    b.Property<int>("ShopId")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("PayoutStatusId");

                    b.HasIndex("ShopId");

                    b.ToTable("Payouts");
                });

            modelBuilder.Entity("Mps.Domain.Entities.PayoutStatus", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("PayoutStatuses");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Pending"
                        },
                        new
                        {
                            Id = 2,
                            Name = "Success"
                        },
                        new
                        {
                            Id = 3,
                            Name = "Failed"
                        });
                });

            modelBuilder.Entity("Mps.Domain.Entities.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("BrandId")
                        .HasColumnType("integer");

                    b.Property<int>("CategoryId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<decimal>("Price")
                        .HasColumnType("numeric");

                    b.Property<int>("ShopId")
                        .HasColumnType("integer");

                    b.Property<int>("SoldCount")
                        .HasColumnType("integer");

                    b.Property<int>("Stock")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("ViewCount")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("BrandId");

                    b.HasIndex("CategoryId");

                    b.HasIndex("ShopId");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("Mps.Domain.Entities.ProductBrand", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("ProductBrands");
                });

            modelBuilder.Entity("Mps.Domain.Entities.ProductCategory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int?>("ParentId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ParentId");

                    b.ToTable("ProductCategories");
                });

            modelBuilder.Entity("Mps.Domain.Entities.ProductImage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ImagePath")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("ProductId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.ToTable("ProductImages");
                });

            modelBuilder.Entity("Mps.Domain.Entities.Shop", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Avatar")
                        .HasColumnType("text");

                    b.Property<string>("City")
                        .HasColumnType("text");

                    b.Property<string>("Cover")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("District")
                        .HasColumnType("text");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<double?>("Latitude")
                        .HasColumnType("double precision");

                    b.Property<double?>("Longitude")
                        .HasColumnType("double precision");

                    b.Property<string>("PayPalAccount")
                        .HasColumnType("text");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ShopName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("ShopOwnerId")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("ShopOwnerId");

                    b.ToTable("Shops");
                });

            modelBuilder.Entity("Mps.Domain.Entities.ShopOwner", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("UserId");

                    b.ToTable("ShopOwners");
                });

            modelBuilder.Entity("Mps.Domain.Entities.Staff", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.Property<string>("Address")
                        .HasColumnType("text");

                    b.Property<string>("CertificatePath")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("IdentityCard")
                        .HasColumnType("text");

                    b.Property<string>("IdentityCardBackPath")
                        .HasColumnType("text");

                    b.Property<string>("IdentityCardFrontPath")
                        .HasColumnType("text");

                    b.Property<string>("StaffCode")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("text")
                        .HasDefaultValueSql("generate_staff_code()");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("UserId");

                    b.ToTable("Staff");
                });

            modelBuilder.Entity("Mps.Domain.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("AvatarPath")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("IdentityId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("text");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("IdentityId")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Mps.Domain.Entities.UserDevice", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<float?>("DeviceLatitude")
                        .HasColumnType("real");

                    b.Property<float?>("DeviceLongitude")
                        .HasColumnType("real");

                    b.Property<string>("DeviceName")
                        .HasColumnType("text");

                    b.Property<string>("DeviceToken")
                        .HasColumnType("text");

                    b.Property<bool>("IsLogged")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserDevices");
                });

            modelBuilder.Entity("Mps.Domain.Entities.Customer", b =>
                {
                    b.HasOne("Mps.Domain.Entities.User", null)
                        .WithOne("Customer")
                        .HasForeignKey("Mps.Domain.Entities.Customer", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Mps.Domain.Entities.Order", b =>
                {
                    b.HasOne("Mps.Domain.Entities.Customer", "Customer")
                        .WithMany()
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Mps.Domain.Entities.OrderStatus", "OrderStatus")
                        .WithMany()
                        .HasForeignKey("OrderStatusId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Mps.Domain.Entities.PaymentMethod", "PaymentMethod")
                        .WithMany()
                        .HasForeignKey("PaymentMethodId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Mps.Domain.Entities.PaymentStatus", "PaymentStatus")
                        .WithMany()
                        .HasForeignKey("PaymentStatusId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Mps.Domain.Entities.Shop", "Shop")
                        .WithMany()
                        .HasForeignKey("ShopId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Customer");

                    b.Navigation("OrderStatus");

                    b.Navigation("PaymentMethod");

                    b.Navigation("PaymentStatus");

                    b.Navigation("Shop");
                });

            modelBuilder.Entity("Mps.Domain.Entities.OrderDetail", b =>
                {
                    b.HasOne("Mps.Domain.Entities.Order", null)
                        .WithMany("OrderDetails")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Mps.Domain.Entities.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");
                });

            modelBuilder.Entity("Mps.Domain.Entities.OrderProgress", b =>
                {
                    b.HasOne("Mps.Domain.Entities.Order", null)
                        .WithMany("Progresses")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Mps.Domain.Entities.Payment", b =>
                {
                    b.HasOne("Mps.Domain.Entities.PaymentSignature", "PaymentSignature")
                        .WithMany()
                        .HasForeignKey("PaymentSignatureId");

                    b.HasOne("Mps.Domain.Entities.PaymentStatus", "PaymentStatus")
                        .WithMany()
                        .HasForeignKey("PaymentStatusId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("PaymentSignature");

                    b.Navigation("PaymentStatus");
                });

            modelBuilder.Entity("Mps.Domain.Entities.PaymentRef", b =>
                {
                    b.HasOne("Mps.Domain.Entities.Payment", null)
                        .WithMany("PaymentRefs")
                        .HasForeignKey("PaymentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Mps.Domain.Entities.Payout", b =>
                {
                    b.HasOne("Mps.Domain.Entities.PayoutStatus", "PayoutStatus")
                        .WithMany()
                        .HasForeignKey("PayoutStatusId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Mps.Domain.Entities.Shop", null)
                        .WithMany("Payouts")
                        .HasForeignKey("ShopId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("PayoutStatus");
                });

            modelBuilder.Entity("Mps.Domain.Entities.Product", b =>
                {
                    b.HasOne("Mps.Domain.Entities.ProductBrand", "Brand")
                        .WithMany()
                        .HasForeignKey("BrandId");

                    b.HasOne("Mps.Domain.Entities.ProductCategory", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Mps.Domain.Entities.Shop", "Shop")
                        .WithMany()
                        .HasForeignKey("ShopId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Brand");

                    b.Navigation("Category");

                    b.Navigation("Shop");
                });

            modelBuilder.Entity("Mps.Domain.Entities.ProductCategory", b =>
                {
                    b.HasOne("Mps.Domain.Entities.ProductCategory", null)
                        .WithMany("Children")
                        .HasForeignKey("ParentId");
                });

            modelBuilder.Entity("Mps.Domain.Entities.ProductImage", b =>
                {
                    b.HasOne("Mps.Domain.Entities.Product", null)
                        .WithMany("Images")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Mps.Domain.Entities.Shop", b =>
                {
                    b.HasOne("Mps.Domain.Entities.ShopOwner", null)
                        .WithMany("Shops")
                        .HasForeignKey("ShopOwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Mps.Domain.Entities.ShopOwner", b =>
                {
                    b.HasOne("Mps.Domain.Entities.User", null)
                        .WithOne("ShopOwner")
                        .HasForeignKey("Mps.Domain.Entities.ShopOwner", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Mps.Domain.Entities.Staff", b =>
                {
                    b.HasOne("Mps.Domain.Entities.User", null)
                        .WithOne("Staff")
                        .HasForeignKey("Mps.Domain.Entities.Staff", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Mps.Domain.Entities.UserDevice", b =>
                {
                    b.HasOne("Mps.Domain.Entities.User", null)
                        .WithMany("UserDevices")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Mps.Domain.Entities.Order", b =>
                {
                    b.Navigation("OrderDetails");

                    b.Navigation("Progresses");
                });

            modelBuilder.Entity("Mps.Domain.Entities.Payment", b =>
                {
                    b.Navigation("PaymentRefs");
                });

            modelBuilder.Entity("Mps.Domain.Entities.Product", b =>
                {
                    b.Navigation("Images");
                });

            modelBuilder.Entity("Mps.Domain.Entities.ProductCategory", b =>
                {
                    b.Navigation("Children");
                });

            modelBuilder.Entity("Mps.Domain.Entities.Shop", b =>
                {
                    b.Navigation("Payouts");
                });

            modelBuilder.Entity("Mps.Domain.Entities.ShopOwner", b =>
                {
                    b.Navigation("Shops");
                });

            modelBuilder.Entity("Mps.Domain.Entities.User", b =>
                {
                    b.Navigation("Customer");

                    b.Navigation("ShopOwner");

                    b.Navigation("Staff");

                    b.Navigation("UserDevices");
                });
#pragma warning restore 612, 618
        }
    }
}
