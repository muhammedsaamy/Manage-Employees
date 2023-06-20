using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Employees_CRUD.Migrations
{
    public partial class FTPSettings3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FTPFileSettings_Employees_EmployeeId",
                table: "FTPFileSettings");

            migrationBuilder.DropIndex(
                name: "IX_FTPFileSettings_EmployeeId",
                table: "FTPFileSettings");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "FTPFileSettings");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EmployeeId",
                table: "FTPFileSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_FTPFileSettings_EmployeeId",
                table: "FTPFileSettings",
                column: "EmployeeId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FTPFileSettings_Employees_EmployeeId",
                table: "FTPFileSettings",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
