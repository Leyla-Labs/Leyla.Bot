using System;
using Common.Enums;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Common.Db.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:discord_entity_type", "channel,role")
                .Annotation("Npgsql:Enum:user_log_type", "warning,silence,ban");

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
                name: "members",
                columns: table => new
                {
                    user_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    guild_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_members", x => new { x.user_id, x.guild_id });
                    table.ForeignKey(
                        name: "fk_members_guilds_guild_id",
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
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_self_assign_menus_guilds_guild_id",
                        column: x => x.guild_id,
                        principalTable: "guilds",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "stashes",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    guild_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    required_role_id = table.Column<decimal>(type: "numeric(20,0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_stashes", x => x.id);
                    table.ForeignKey(
                        name: "fk_stashes_discord_entities_required_role_id",
                        column: x => x.required_role_id,
                        principalTable: "discord_entities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_stashes_guilds_guild_id",
                        column: x => x.guild_id,
                        principalTable: "guilds",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "command_logs",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    command = table.Column<string>(type: "text", nullable: false),
                    run_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    user_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    guild_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_command_logs", x => x.id);
                    table.ForeignKey(
                        name: "fk_command_logs_members_member_temp_id",
                        columns: x => new { x.user_id, x.guild_id },
                        principalTable: "members",
                        principalColumns: new[] { "user_id", "guild_id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "quotes",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    text = table.Column<string>(type: "text", nullable: false),
                    date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    message_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    guild_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    user_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_quotes", x => x.id);
                    table.ForeignKey(
                        name: "fk_quotes_members_member_temp_id1",
                        columns: x => new { x.user_id, x.guild_id },
                        principalTable: "members",
                        principalColumns: new[] { "user_id", "guild_id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_logs",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    member_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    reason = table.Column<string>(type: "text", nullable: false),
                    additional_details = table.Column<string>(type: "text", nullable: false),
                    date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    type = table.Column<UserLogType>(type: "user_log_type", nullable: false),
                    guild_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    author_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_logs", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_logs_members_author_user_id_author_guild_id",
                        columns: x => new { x.member_id, x.guild_id },
                        principalTable: "members",
                        principalColumns: new[] { "user_id", "guild_id" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_logs_members_member_user_id_member_guild_id",
                        columns: x => new { x.author_id, x.guild_id },
                        principalTable: "members",
                        principalColumns: new[] { "user_id", "guild_id" },
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

            migrationBuilder.CreateTable(
                name: "stash_entries",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    value = table.Column<string>(type: "text", nullable: false),
                    stash_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_stash_entries", x => x.id);
                    table.ForeignKey(
                        name: "fk_stash_entries_stashes_stash_id",
                        column: x => x.stash_id,
                        principalTable: "stashes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_command_logs_user_id_guild_id",
                table: "command_logs",
                columns: new[] { "user_id", "guild_id" });

            migrationBuilder.CreateIndex(
                name: "ix_configs_guild_id",
                table: "configs",
                column: "guild_id");

            migrationBuilder.CreateIndex(
                name: "ix_discord_entities_guild_id",
                table: "discord_entities",
                column: "guild_id");

            migrationBuilder.CreateIndex(
                name: "ix_members_guild_id",
                table: "members",
                column: "guild_id");

            migrationBuilder.CreateIndex(
                name: "ix_quotes_user_id_guild_id",
                table: "quotes",
                columns: new[] { "user_id", "guild_id" });

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

            migrationBuilder.CreateIndex(
                name: "ix_stash_entries_stash_id",
                table: "stash_entries",
                column: "stash_id");

            migrationBuilder.CreateIndex(
                name: "ix_stashes_guild_id",
                table: "stashes",
                column: "guild_id");

            migrationBuilder.CreateIndex(
                name: "ix_stashes_required_role_id",
                table: "stashes",
                column: "required_role_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_logs_author_id_guild_id",
                table: "user_logs",
                columns: new[] { "author_id", "guild_id" });

            migrationBuilder.CreateIndex(
                name: "ix_user_logs_member_id_guild_id",
                table: "user_logs",
                columns: new[] { "member_id", "guild_id" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "command_logs");

            migrationBuilder.DropTable(
                name: "configs");

            migrationBuilder.DropTable(
                name: "quotes");

            migrationBuilder.DropTable(
                name: "self_assign_menu_discord_entity_assignments");

            migrationBuilder.DropTable(
                name: "stash_entries");

            migrationBuilder.DropTable(
                name: "user_logs");

            migrationBuilder.DropTable(
                name: "self_assign_menus");

            migrationBuilder.DropTable(
                name: "stashes");

            migrationBuilder.DropTable(
                name: "members");

            migrationBuilder.DropTable(
                name: "discord_entities");

            migrationBuilder.DropTable(
                name: "guilds");
        }
    }
}
