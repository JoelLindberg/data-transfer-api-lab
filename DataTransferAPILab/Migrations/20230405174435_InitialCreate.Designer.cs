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
    [Migration("20230405174435_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.4");

            modelBuilder.Entity("DataTransferApiLab.Models.Transfer", b =>
                {
                    b.Property<int>("TransferId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("TransferData")
                        .HasColumnType("TEXT");

                    b.HasKey("TransferId");

                    b.ToTable("Transfers");
                });
#pragma warning restore 612, 618
        }
    }
}