﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using N17Solutions.Semaphore.Data.Context;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace N17Solutions.Semaphore.Data.Migrations
{
    [DbContext(typeof(SemaphoreContext))]
    [Migration("20180926221655_Add-IsBaseType-Column")]
    partial class AddIsBaseTypeColumn
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.1.3-rtm-32065")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("N17Solutions.Semaphore.Domain.Model.Feature", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime>("DateLastUpdated");

                    b.Property<string>("Name");

                    b.Property<Guid>("ResourceId");

                    b.HasKey("Id");

                    b.ToTable("Feature");
                });

            modelBuilder.Entity("N17Solutions.Semaphore.Domain.Model.Setting", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("Id");

                    b.ToTable("Setting");
                });

            modelBuilder.Entity("N17Solutions.Semaphore.Domain.Model.Signal", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime>("DateLastUpdated");

                    b.Property<long?>("FeatureId");

                    b.Property<bool>("IsBaseType");

                    b.Property<string>("Name");

                    b.Property<Guid?>("ResourceId");

                    b.Property<string>("Tags");

                    b.Property<string>("Value");

                    b.Property<string>("ValueType");

                    b.HasKey("Id");

                    b.HasIndex("FeatureId");

                    b.ToTable("Signal");
                });

            modelBuilder.Entity("N17Solutions.Semaphore.Domain.Model.Signal", b =>
                {
                    b.HasOne("N17Solutions.Semaphore.Domain.Model.Feature")
                        .WithMany("Signals")
                        .HasForeignKey("FeatureId");
                });
#pragma warning restore 612, 618
        }
    }
}