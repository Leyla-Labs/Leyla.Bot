using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Db.Migrations
{
    public partial class UserLogAdditionalDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "text",
                table: "user_logs",
                newName: "reason");

            migrationBuilder.AddColumn<string>(
                name: "additional_details",
                table: "user_logs",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "additional_details",
                table: "user_logs");

            migrationBuilder.RenameColumn(
                name: "reason",
                table: "user_logs",
                newName: "text");
        }
    }
}
