

#nullable disable

using Common.Enums;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
namespace Db.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:discord_entity_type", "channel,role");

            migrationBuilder.CreateTable(
                name: "guilds",
                columns: table => new
                {
                    id = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_guilds", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "configs",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    config_option_id = table.Column<int>(type: "integer", nullable: false),
                    guild_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_configs", x => x.id);
                    table.ForeignKey(
                        name: "fk_configs_guilds_guild_id",
                        column: x => x.guild_id,
                        principalTable: "guilds",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "discord_entities",
                columns: table => new
                {
                    id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    guild_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    discord_entity_type = table.Column<DiscordEntityType>(type: "discord_entity_type", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_discord_entities", x => x.id);
                    table.ForeignKey(
                        name: "fk_discord_entities_guilds_guild_id",
                        column: x => x.guild_id,
                        principalTable: "guilds",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "self_assign_menus",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    title = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    guild_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    required_role_id = table.Column<decimal>(type: "numeric(20,0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_self_assign_menus", x => x.id);
                    table.ForeignKey(
                        name: "fk_self_assign_menus_discord_entities_required_role_id",
                        column: x => x.required_role_id,
                        principalTable: "discord_entities",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_self_assign_menus_guilds_guild_id",
                        column: x => x.guild_id,
                        principalTable: "guilds",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "self_assign_menu_discord_entity_assignments",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    self_assign_menu_id = table.Column<int>(type: "integer", nullable: false),
                    discord_entity_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_self_assign_menu_discord_entity_assignments", x => x.id);
                    table.ForeignKey(
                        name: "fk_self_assign_menu_discord_entity_assignments_discord_entitie",
                        column: x => x.discord_entity_id,
                        principalTable: "discord_entities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_self_assign_menu_discord_entity_assignments_self_assign_men",
                        column: x => x.self_assign_menu_id,
                        principalTable: "self_assign_menus",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_configs_guild_id",
                table: "configs",
                column: "guild_id");

            migrationBuilder.CreateIndex(
                name: "ix_discord_entities_guild_id",
                table: "discord_entities",
                column: "guild_id");

            migrationBuilder.CreateIndex(
                name: "ix_self_assign_menu_discord_entity_assignments_discord_entity_",
                table: "self_assign_menu_discord_entity_assignments",
                column: "discord_entity_id");

            migrationBuilder.CreateIndex(
                name: "ix_self_assign_menu_discord_entity_assignments_self_assign_men",
                table: "self_assign_menu_discord_entity_assignments",
                column: "self_assign_menu_id");

            migrationBuilder.CreateIndex(
                name: "ix_self_assign_menus_guild_id",
                table: "self_assign_menus",
                column: "guild_id");

            migrationBuilder.CreateIndex(
                name: "ix_self_assign_menus_required_role_id",
                table: "self_assign_menus",
                column: "required_role_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "configs");

            migrationBuilder.DropTable(
                name: "self_assign_menu_discord_entity_assignments");

            migrationBuilder.DropTable(
                name: "self_assign_menus");

            migrationBuilder.DropTable(
                name: "discord_entities");

            migrationBuilder.DropTable(
                name: "guilds");
        }
    }
}
