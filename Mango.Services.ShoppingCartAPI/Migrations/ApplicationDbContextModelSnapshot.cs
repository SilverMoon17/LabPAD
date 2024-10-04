﻿// <auto-generated />
using Mango.Services.ShoppingCartAPI.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Mango.Services.ShoppingCartAPI.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.31")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Mango.Services.ShoppingCartAPI.Models.CartDetails", b =>
                {
                    b.Property<int>("CartDetailsId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("CartDetailsId"));

                    b.Property<int>("CartHeaderId")
                        .HasColumnType("integer");

                    b.Property<int>("Count")
                        .HasColumnType("integer");

                    b.Property<int>("ProductId")
                        .HasColumnType("integer");

                    b.HasKey("CartDetailsId");

                    b.HasIndex("CartHeaderId");

                    b.HasIndex("ProductId");

                    b.ToTable("CartDetails");
                });

            modelBuilder.Entity("Mango.Services.ShoppingCartAPI.Models.CartHeader", b =>
                {
                    b.Property<int>("CartHeaderId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("CartHeaderId"));

                    b.Property<string>("CouponCode")
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.HasKey("CartHeaderId");

                    b.ToTable("CartHeaders");
                });

            modelBuilder.Entity("Mango.Services.ShoppingCartAPI.Models.Product", b =>
                {
                    b.Property<int>("ProductId")
                        .HasColumnType("integer");

                    b.Property<string>("CategoryName")
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("ImageUrl")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<double>("Price")
                        .HasColumnType("double precision");

                    b.HasKey("ProductId");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("Mango.Services.ShoppingCartAPI.Models.CartDetails", b =>
                {
                    b.HasOne("Mango.Services.ShoppingCartAPI.Models.CartHeader", "CartHeader")
                        .WithMany()
                        .HasForeignKey("CartHeaderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Mango.Services.ShoppingCartAPI.Models.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CartHeader");

                    b.Navigation("Product");
                });
#pragma warning restore 612, 618
        }
    }
}
