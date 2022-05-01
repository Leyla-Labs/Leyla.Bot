using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Db.Migrations
{
    public partial class QuotesNamingFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_member_guilds_guild_id",
                table: "member");

            migrationBuilder.DropForeignKey(
                name: "fk_quote_member_member_id",
                table: "quote");

            migrationBuilder.DropPrimaryKey(
                name: "pk_quote",
                table: "quote");

            migrationBuilder.DropPrimaryKey(
                name: "pk_member",
                table: "member");

            migrationBuilder.RenameTable(
                name: "quote",
                newName: "quotes");

            migrationBuilder.RenameTable(
                name: "member",
                newName: "members");

            migrationBuilder.RenameIndex(
                name: "ix_quote_member_id",
                table: "quotes",
                newName: "ix_quotes_member_id");

            migrationBuilder.RenameIndex(
                name: "ix_member_guild_id",
                table: "members",
                newName: "ix_members_guild_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_quotes",
                table: "quotes",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_members",
                table: "members",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_members_guilds_guild_id",
                table: "members",
                column: "guild_id",
                principalTable: "guilds",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_quotes_members_member_id",
                table: "quotes",
                column: "member_id",
                principalTable: "members",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_members_guilds_guild_id",
                table: "members");

            migrationBuilder.DropForeignKey(
                name: "fk_quotes_members_member_id",
                table: "quotes");

            migrationBuilder.DropPrimaryKey(
                name: "pk_quotes",
                table: "quotes");

            migrationBuilder.DropPrimaryKey(
                name: "pk_members",
                table: "members");

            migrationBuilder.RenameTable(
                name: "quotes",
                newName: "quote");

            migrationBuilder.RenameTable(
                name: "members",
                newName: "member");

            migrationBuilder.RenameIndex(
                name: "ix_quotes_member_id",
                table: "quote",
                newName: "ix_quote_member_id");

            migrationBuilder.RenameIndex(
                name: "ix_members_guild_id",
                table: "member",
                newName: "ix_member_guild_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_quote",
                table: "quote",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_member",
                table: "member",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_member_guilds_guild_id",
                table: "member",
                column: "guild_id",
                principalTable: "guilds",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_quote_member_member_id",
                table: "quote",
                column: "member_id",
                principalTable: "member",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
