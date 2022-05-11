using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Db.Migrations
{
    public partial class Stashes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_stashes_guilds_guild_id",
                        column: x => x.guild_id,
                        principalTable: "guilds",
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "stash_entries");

            migrationBuilder.DropTable(
                name: "stashes");
        }
    }
}
