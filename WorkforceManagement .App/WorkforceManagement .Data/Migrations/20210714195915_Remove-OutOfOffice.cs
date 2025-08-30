using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkforceManagement.Data.Migrations
{
    public partial class RemoveOutOfOffice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OutOfOffice",
                table: "AspNetUsers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeSpan>(
                name: "OutOfOffice",
                table: "AspNetUsers",
                type: "time",
                nullable: true);
        }
    }
}
