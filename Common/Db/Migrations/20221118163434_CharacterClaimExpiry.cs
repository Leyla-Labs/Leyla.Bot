using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Common.Db.Migrations
{
    public partial class CharacterClaimExpiry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_character_claim",
                table: "character_claim");

            migrationBuilder.RenameTable(
                name: "character_claim",
                newName: "character_claims");

            migrationBuilder.RenameIndex(
                name: "ix_character_claim_code",
                table: "character_claims",
                newName: "ix_character_claims_code");

            migrationBuilder.RenameIndex(
                name: "ix_character_claim_character_id",
                table: "character_claims",
                newName: "ix_character_claims_character_id");

            migrationBuilder.AddColumn<DateTime>(
                name: "valid_until",
                table: "character_claims",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "pk_character_claims",
                table: "character_claims",
                column: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_character_claims",
                table: "character_claims");

            migrationBuilder.DropColumn(
                name: "valid_until",
                table: "character_claims");

            migrationBuilder.RenameTable(
                name: "character_claims",
                newName: "character_claim");

            migrationBuilder.RenameIndex(
                name: "ix_character_claims_code",
                table: "character_claim",
                newName: "ix_character_claim_code");

            migrationBuilder.RenameIndex(
                name: "ix_character_claims_character_id",
                table: "character_claim",
                newName: "ix_character_claim_character_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_character_claim",
                table: "character_claim",
                column: "id");
        }
    }
}
