﻿// <auto-generated />
using DataTransferApiLab.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DataTransferApiLab.Migrations
{
    [DbContext(typeof(DataTransferApiLabContext))]
    [Migration("20230406185653_AuditsCreate")]
    partial class AuditsCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.4");

            modelBuilder.Entity("DataTransferApiLab.Models.Audit", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Date")
                        .HasColumnType("TEXT");

                    b.Property<string>("Event")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Audits");
                });

            modelBuilder.Entity("DataTransferApiLab.Models.Transfer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("TransferData")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Transfers");
                });
#pragma warning restore 612, 618
        }
    }
}