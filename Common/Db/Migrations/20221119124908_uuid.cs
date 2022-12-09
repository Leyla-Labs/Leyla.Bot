using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Common.Db.Migrations
{
    public partial class Uuid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:discord_entity_type", "channel,role")
                .Annotation("Npgsql:Enum:user_log_type", "warning,silence,ban")
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,")
                .OldAnnotation("Npgsql:Enum:discord_entity_type", "channel,role")
                .OldAnnotation("Npgsql:Enum:user_log_type", "warning,silence,ban");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:discord_entity_type", "channel,role")
                .Annotation("Npgsql:Enum:user_log_type", "warning,silence,ban")
                .OldAnnotation("Npgsql:Enum:discord_entity_type", "channel,role")
                .OldAnnotation("Npgsql:Enum:user_log_type", "warning,silence,ban")
                .OldAnnotation("Npgsql:PostgresExtension:uuid-ossp", ",,");
        }
    }
}
