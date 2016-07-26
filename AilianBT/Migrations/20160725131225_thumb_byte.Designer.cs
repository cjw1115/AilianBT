using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations;
using BtDownload.Models;

namespace BtDownload.Migrations
{
    [DbContext(typeof(DownloadDbContext))]
    [Migration("20160725131225_thumb_byte")]
    partial class thumb_byte
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0-rc1-16348");

            modelBuilder.Entity("BtDownload.DownloadedInfo", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DownloadedTime");

                    b.Property<string>("FileName");

                    b.Property<string>("FilePath");

                    b.Property<ulong>("Size");

                    b.Property<byte[]>("Thumb");

                    b.HasKey("ID");
                });
        }
    }
}
