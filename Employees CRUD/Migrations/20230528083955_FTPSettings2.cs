using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Employees_CRUD.Migrations
{
    public partial class FTPSettings2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FilePath",
                table: "FTPFileSettings",
                newName: "FTP_Server");

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileName",
                table: "Employees");

            migrationBuilder.RenameColumn(
                name: "FTP_Server",
                table: "FTPFileSettings",
                newName: "FilePath");
        }
    }
}
