using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Db.Migrations
{
    public partial class UuidExtension : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // necessary to get random entries using Guid.NewGuid()
            migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS \"uuid-ossp\";");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
