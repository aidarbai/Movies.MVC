using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Cinema.DAL.Migrations
{
    public partial class AddedBannedOnDateUSerProperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "BannedOnDate",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BannedOnDate",
                table: "AspNetUsers");
        }
    }
}
