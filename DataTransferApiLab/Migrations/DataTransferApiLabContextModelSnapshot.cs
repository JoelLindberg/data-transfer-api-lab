﻿// <auto-generated />
using DataTransferApiLab.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DataTransferAPILab.Migrations
{
    [DbContext(typeof(DataTransferApiLabContext))]
    partial class DataTransferApiLabContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.4");

            modelBuilder.Entity("DataTransferApiLab.Models.Audit", b =>
                {
                    b.Property<int>("AuditId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Action")
                        .IsRequired()
                        .HasColumnType("nvarchar(20)");

                    b.Property<int>("Bytes")
                        .HasColumnType("int");

                    b.Property<string>("Timestamp")
                        .IsRequired()
                        .HasColumnType("nvarchar(19)");

                    b.Property<int>("TransferDataId")
                        .HasColumnType("int");

                    b.Property<string>("TransferName")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.HasKey("AuditId");

                    b.ToTable("Audits");
                });

            modelBuilder.Entity("DataTransferApiLab.Models.Transfer", b =>
                {
                    b.Property<int>("TransferDataId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Bytes")
                        .HasColumnType("int");

                    b.Property<string>("TransferData")
                        .IsRequired()
                        .HasColumnType("varchar(500000)");

                    b.Property<string>("TransferName")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.HasKey("TransferDataId");

                    b.ToTable("Transfers");
                });
#pragma warning restore 612, 618
        }
    }
}
