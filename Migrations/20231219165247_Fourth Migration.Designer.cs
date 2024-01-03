﻿// <auto-generated />
using System;
using AssetLoaningApplication.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace AssetLoaningApplication.Migrations
{
    [DbContext(typeof(AssetLoanDbContext))]
    [Migration("20231219165247_Fourth Migration")]
    partial class FourthMigration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("AssetLoaningApplication.Models.Domain.AssetDetails", b =>
                {
                    b.Property<Guid>("assetId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("model")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("serialNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("assetId");

                    b.ToTable("AssetDetails");
                });

            modelBuilder.Entity("AssetLoaningApplication.Models.Domain.TransactionDetails", b =>
                {
                    b.Property<Guid>("transactionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("assetDetailsassetId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateOnly>("date")
                        .HasColumnType("date");

                    b.Property<string>("loanedOrReturned")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("transactionType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("transactionId");

                    b.HasIndex("assetDetailsassetId");

                    b.ToTable("TransactionDetails");
                });

            modelBuilder.Entity("AssetLoaningApplication.Models.Domain.UserDetails", b =>
                {
                    b.Property<Guid>("userId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("TransactionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("firstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("gender")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("lastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("role")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("userId");

                    b.ToTable("UserDetails");
                });

            modelBuilder.Entity("AssetLoaningApplication.Models.Domain.TransactionDetails", b =>
                {
                    b.HasOne("AssetLoaningApplication.Models.Domain.AssetDetails", "assetDetails")
                        .WithMany()
                        .HasForeignKey("assetDetailsassetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("assetDetails");
                });

            modelBuilder.Entity("AssetLoaningApplication.Models.Domain.UserDetails", b =>
                {
                    b.HasOne("AssetLoaningApplication.Models.Domain.TransactionDetails", "TransactionDetails")
                        .WithMany("UserDetails")
                        .HasForeignKey("userId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("TransactionDetails");
                });

            modelBuilder.Entity("AssetLoaningApplication.Models.Domain.TransactionDetails", b =>
                {
                    b.Navigation("UserDetails");
                });
#pragma warning restore 612, 618
        }
    }
}
