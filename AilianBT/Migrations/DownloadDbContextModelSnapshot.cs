﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using BtDownload.Models;

namespace AilianBT.Migrations
{
    [DbContext(typeof(DownloadDbContext))]
    partial class DownloadDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.0-rtm-21431");

            modelBuilder.Entity("BtDownload.Models.DownloadedInfo", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DownloadedTime");

                    b.Property<string>("FileName");

                    b.Property<string>("FilePath");

                    b.Property<ulong>("Size");

                    b.Property<byte[]>("Thumb");

                    b.HasKey("ID");

                    b.ToTable("DownloadedInfos");
                });
        }
    }
}
