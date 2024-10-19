﻿// <auto-generated />
using System;
using DocumentService.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DocumentService.Data.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20241019152740_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("DocumentService.Models.History.History", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Data")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("data");

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("date");

                    b.Property<string>("DoctorId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("doctor_id");

                    b.Property<int>("HospitalId")
                        .HasColumnType("integer")
                        .HasColumnName("hospital_id");

                    b.Property<string>("PacientId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("pacient_id");

                    b.Property<string>("Room")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("room");

                    b.HasKey("Id")
                        .HasName("pk_histories");

                    b.ToTable("histories", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
