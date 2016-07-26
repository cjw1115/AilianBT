using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;

namespace BtDownload.Migrations
{
    public partial class thumb_byte : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Thumb",
                table: "DownloadedInfo",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "Thumb", table: "DownloadedInfo");
        }
    }
}
