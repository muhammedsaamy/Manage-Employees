using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Employees_CRUD.Migrations
{
    public partial class FTPSettings4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FileName",
                table: "Employees",
                newName: "File");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "File",
                table: "Employees",
                newName: "FileName");
        }
    }
}
