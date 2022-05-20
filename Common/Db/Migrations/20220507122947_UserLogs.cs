

#nullable disable

using System;
using Common.Enums;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
namespace Db.Migrations
{
    public partial class UserLogs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_self_assign_menus_discord_entities_required_role_id",
                table: "self_assign_menus");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:discord_entity_type", "channel,role")
                .Annotation("Npgsql:Enum:user_log_type", "warning,silence,ban")
                .OldAnnotation("Npgsql:Enum:discord_entity_type", "channel,role");

            migrationBuilder.CreateTable(
                name: "user_logs",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    member_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    text = table.Column<string>(type: "text", nullable: false),
                    date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    type = table.Column<UserLogType>(type: "user_log_type", nullable: false),
                    author_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_logs", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_logs_members_author_id",
                        column: x => x.author_id,
                        principalTable: "members",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_logs_members_member_id",
                        column: x => x.member_id,
                        principalTable: "members",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_user_logs_author_id",
                table: "user_logs",
                column: "author_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_logs_member_id",
                table: "user_logs",
                column: "member_id");

            migrationBuilder.AddForeignKey(
                name: "fk_self_assign_menus_discord_entities_required_role_id",
                table: "self_assign_menus",
                column: "required_role_id",
                principalTable: "discord_entities",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_self_assign_menus_discord_entities_required_role_id",
                table: "self_assign_menus");

            migrationBuilder.DropTable(
                name: "user_logs");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:discord_entity_type", "channel,role")
                .OldAnnotation("Npgsql:Enum:discord_entity_type", "channel,role")
                .OldAnnotation("Npgsql:Enum:user_log_type", "warning,silence,ban");

            migrationBuilder.AddForeignKey(
                name: "fk_self_assign_menus_discord_entities_required_role_id",
                table: "self_assign_menus",
                column: "required_role_id",
                principalTable: "discord_entities",
                principalColumn: "id");
        }
    }
}
