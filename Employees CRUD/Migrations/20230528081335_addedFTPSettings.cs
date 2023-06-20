using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Employees_CRUD.Migrations
{
    public partial class addedFTPSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FTPFileSettings",
                columns: table => new
                {
                    FTPFileSettingsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FTP_Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FTP_Password = table.Column<int>(type: "int", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FTPFileSettings", x => x.FTPFileSettingsId);
                    table.ForeignKey(
                        name: "FK_FTPFileSettings_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FTPFileSettings_EmployeeId",
                table: "FTPFileSettings",
                column: "EmployeeId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FTPFileSettings");
        }
    }
}
