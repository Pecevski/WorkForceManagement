using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkforceManagement.Data.Migrations
{
    public partial class AddDaysOffToUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TimeOffRequests_IsDeleted",
                table: "TimeOffRequests");

            migrationBuilder.DropIndex(
                name: "IX_Teams_IsDeleted",
                table: "Teams");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_IsDeleted",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_Approvals_IsDeleted",
                table: "Approvals");

            migrationBuilder.AddColumn<int>(
                name: "PaidDaysOff",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SickDaysOff",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UnpaidDaysOff",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaidDaysOff",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SickDaysOff",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UnpaidDaysOff",
                table: "AspNetUsers");

            migrationBuilder.CreateIndex(
                name: "IX_TimeOffRequests_IsDeleted",
                table: "TimeOffRequests",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_IsDeleted",
                table: "Teams",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_IsDeleted",
                table: "AspNetUsers",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Approvals_IsDeleted",
                table: "Approvals",
                column: "IsDeleted");
        }
    }
}
