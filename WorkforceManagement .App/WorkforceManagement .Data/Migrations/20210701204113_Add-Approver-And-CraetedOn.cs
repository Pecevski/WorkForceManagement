using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkforceManagement.Data.Migrations
{
    public partial class AddApproverAndCraetedOn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Approvals_AspNetUsers_UserId",
                table: "Approvals");

            migrationBuilder.DropIndex(
                name: "IX_Approvals_UserId",
                table: "Approvals");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Approvals");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "TimeOffRequests",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ApproverId",
                table: "Approvals",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Approvals_ApproverId",
                table: "Approvals",
                column: "ApproverId");

            migrationBuilder.AddForeignKey(
                name: "FK_Approvals_AspNetUsers_ApproverId",
                table: "Approvals",
                column: "ApproverId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Approvals_AspNetUsers_ApproverId",
                table: "Approvals");

            migrationBuilder.DropIndex(
                name: "IX_Approvals_ApproverId",
                table: "Approvals");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "TimeOffRequests");

            migrationBuilder.DropColumn(
                name: "ApproverId",
                table: "Approvals");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Approvals",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Approvals_UserId",
                table: "Approvals",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Approvals_AspNetUsers_UserId",
                table: "Approvals",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
