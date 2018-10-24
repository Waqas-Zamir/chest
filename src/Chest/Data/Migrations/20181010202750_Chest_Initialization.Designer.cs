﻿// <auto-generated />
using Chest.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Chest.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20181010202750_Chest_Initialization")]
    partial class Chest_Initialization
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.0-rtm-30799")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Chest.Data.KeyValueData", b =>
                {
                    b.Property<string>("Category")
                        .HasMaxLength(100);

                    b.Property<string>("Collection")
                        .HasMaxLength(100);

                    b.Property<string>("Key")
                        .HasMaxLength(100);

                    b.Property<string>("DisplayCategory")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("DisplayCollection")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("DisplayKey")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("Keywords")
                        .HasMaxLength(1024);

                    b.Property<string>("MetaData")
                        .IsRequired()
                        .HasMaxLength(4096);

                    b.HasKey("Category", "Collection", "Key");

                    b.ToTable("tb_keyValueData","chest");
                });
#pragma warning restore 612, 618
        }
    }
}