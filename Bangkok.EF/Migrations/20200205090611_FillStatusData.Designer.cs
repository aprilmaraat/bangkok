﻿// <auto-generated />
using System;
using Bangkok.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Bangkok.EF.Migrations
{
    [DbContext(typeof(BangkokContext))]
    [Migration("20200205090611_FillStatusData")]
    partial class FillStatusData
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Bangkok.Models.EnumStatus", b =>
                {
                    b.Property<byte>("ID")
                        .HasColumnType("tinyint");

                    b.Property<string>("StatusInitial")
                        .IsRequired()
                        .HasColumnType("nvarchar(1)");

                    b.HasKey("ID");

                    b.ToTable("enum.Status");
                });

            modelBuilder.Entity("Bangkok.Models.TransactionData", b =>
                {
                    b.Property<string>("ID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal");

                    b.Property<string>("CurrencyCode")
                        .HasColumnType("nvarchar(10)");

                    b.Property<byte>("Status")
                        .HasColumnType("tinyint");

                    b.Property<DateTime>("TransactionDT")
                        .HasColumnType("datetime2");

                    b.HasKey("ID");

                    b.HasIndex("Status")
                        .IsUnique();

                    b.ToTable("Transaction.Data");
                });

            modelBuilder.Entity("Bangkok.Models.TransactionData", b =>
                {
                    b.HasOne("Bangkok.Models.EnumStatus", "EnumStatus")
                        .WithOne()
                        .HasForeignKey("Bangkok.Models.TransactionData", "Status")
                        .HasConstraintName("FK_Transaction.Data_enum.Status")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
