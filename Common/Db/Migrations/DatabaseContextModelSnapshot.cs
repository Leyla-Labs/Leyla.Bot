﻿// <auto-generated />
using System;
using Common.Db;
using Common.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Common.Db.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    partial class DatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.HasPostgresEnum(modelBuilder, "discord_entity_type", new[] { "channel", "role" });
            NpgsqlModelBuilderExtensions.HasPostgresEnum(modelBuilder, "user_log_type", new[] { "warning", "silence", "ban" });
            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Common.Db.Models.CharacterClaim", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("CharacterId")
                        .HasColumnType("integer")
                        .HasColumnName("character_id");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("code");

                    b.Property<bool>("Confirmed")
                        .HasColumnType("boolean")
                        .HasColumnName("confirmed");

                    b.Property<decimal>("UserId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("user_id");

                    b.Property<DateTime>("ValidUntil")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("valid_until");

                    b.HasKey("Id")
                        .HasName("pk_character_claims");

                    b.HasIndex("CharacterId")
                        .IsUnique()
                        .HasDatabaseName("ix_character_claims_character_id");

                    b.HasIndex("Code")
                        .IsUnique()
                        .HasDatabaseName("ix_character_claims_code");

                    b.ToTable("character_claims", (string)null);
                });

            modelBuilder.Entity("Common.Db.Models.CommandLog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Command")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("command");

                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("guild_id");

                    b.Property<DateTime>("RunAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("run_at");

                    b.Property<decimal>("UserId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_command_logs");

                    b.HasIndex("UserId", "GuildId")
                        .HasDatabaseName("ix_command_logs_user_id_guild_id");

                    b.ToTable("command_logs", (string)null);
                });

            modelBuilder.Entity("Common.Db.Models.Config", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("ConfigOptionId")
                        .HasColumnType("integer")
                        .HasColumnName("config_option_id");

                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("guild_id");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("value");

                    b.HasKey("Id")
                        .HasName("pk_configs");

                    b.HasIndex("GuildId")
                        .HasDatabaseName("ix_configs_guild_id");

                    b.ToTable("configs", (string)null);
                });

            modelBuilder.Entity("Common.Db.Models.DiscordEntity", b =>
                {
                    b.Property<decimal>("Id")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("id");

                    b.Property<DiscordEntityType>("DiscordEntityType")
                        .HasColumnType("discord_entity_type")
                        .HasColumnName("discord_entity_type");

                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("guild_id");

                    b.HasKey("Id")
                        .HasName("pk_discord_entities");

                    b.HasIndex("GuildId")
                        .HasDatabaseName("ix_discord_entities_guild_id");

                    b.ToTable("discord_entities", (string)null);
                });

            modelBuilder.Entity("Common.Db.Models.Guild", b =>
                {
                    b.Property<decimal>("Id")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("id");

                    b.HasKey("Id")
                        .HasName("pk_guilds");

                    b.ToTable("guilds", (string)null);
                });

            modelBuilder.Entity("Common.Db.Models.Member", b =>
                {
                    b.Property<decimal>("UserId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("user_id");

                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("guild_id");

                    b.HasKey("UserId", "GuildId")
                        .HasName("pk_members");

                    b.HasIndex("GuildId")
                        .HasDatabaseName("ix_members_guild_id");

                    b.ToTable("members", (string)null);
                });

            modelBuilder.Entity("Common.Db.Models.Quote", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("date");

                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("guild_id");

                    b.Property<decimal>("MessageId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("message_id");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("text");

                    b.Property<decimal>("UserId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_quotes");

                    b.HasIndex("UserId", "GuildId")
                        .HasDatabaseName("ix_quotes_user_id_guild_id");

                    b.ToTable("quotes", (string)null);
                });

            modelBuilder.Entity("Common.Db.Models.SelfAssignMenu", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("guild_id");

                    b.Property<decimal?>("RequiredRoleId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("required_role_id");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("title");

                    b.HasKey("Id")
                        .HasName("pk_self_assign_menus");

                    b.HasIndex("GuildId")
                        .HasDatabaseName("ix_self_assign_menus_guild_id");

                    b.HasIndex("RequiredRoleId")
                        .HasDatabaseName("ix_self_assign_menus_required_role_id");

                    b.ToTable("self_assign_menus", (string)null);
                });

            modelBuilder.Entity("Common.Db.Models.SelfAssignMenuDiscordEntityAssignment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<decimal>("DiscordEntityId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("discord_entity_id");

                    b.Property<int>("SelfAssignMenuId")
                        .HasColumnType("integer")
                        .HasColumnName("self_assign_menu_id");

                    b.HasKey("Id")
                        .HasName("pk_self_assign_menu_discord_entity_assignments");

                    b.HasIndex("DiscordEntityId")
                        .HasDatabaseName("ix_self_assign_menu_discord_entity_assignments_discord_entity_");

                    b.HasIndex("SelfAssignMenuId")
                        .HasDatabaseName("ix_self_assign_menu_discord_entity_assignments_self_assign_men");

                    b.ToTable("self_assign_menu_discord_entity_assignments", (string)null);
                });

            modelBuilder.Entity("Common.Db.Models.Stash", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("guild_id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<decimal?>("RequiredRoleId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("required_role_id");

                    b.HasKey("Id")
                        .HasName("pk_stashes");

                    b.HasIndex("GuildId")
                        .HasDatabaseName("ix_stashes_guild_id");

                    b.HasIndex("RequiredRoleId")
                        .HasDatabaseName("ix_stashes_required_role_id");

                    b.ToTable("stashes", (string)null);
                });

            modelBuilder.Entity("Common.Db.Models.StashEntry", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("StashId")
                        .HasColumnType("integer")
                        .HasColumnName("stash_id");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("value");

                    b.HasKey("Id")
                        .HasName("pk_stash_entries");

                    b.HasIndex("StashId")
                        .HasDatabaseName("ix_stash_entries_stash_id");

                    b.ToTable("stash_entries", (string)null);
                });

            modelBuilder.Entity("Common.Db.Models.UserLog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("AdditionalDetails")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("additional_details");

                    b.Property<decimal>("AuthorId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("author_id");

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("date");

                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("guild_id");

                    b.Property<decimal>("MemberId")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("member_id");

                    b.Property<string>("Reason")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("reason");

                    b.Property<UserLogType>("Type")
                        .HasColumnType("user_log_type")
                        .HasColumnName("type");

                    b.HasKey("Id")
                        .HasName("pk_user_logs");

                    b.HasIndex("AuthorId", "GuildId")
                        .HasDatabaseName("ix_user_logs_author_id_guild_id");

                    b.HasIndex("MemberId", "GuildId")
                        .HasDatabaseName("ix_user_logs_member_id_guild_id");

                    b.ToTable("user_logs", (string)null);
                });

            modelBuilder.Entity("Common.Db.Models.CommandLog", b =>
                {
                    b.HasOne("Common.Db.Models.Member", "Member")
                        .WithMany("CommandLogs")
                        .HasForeignKey("UserId", "GuildId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_command_logs_members_member_temp_id");

                    b.Navigation("Member");
                });

            modelBuilder.Entity("Common.Db.Models.Config", b =>
                {
                    b.HasOne("Common.Db.Models.Guild", "Guild")
                        .WithMany("Configs")
                        .HasForeignKey("GuildId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_configs_guilds_guild_id");

                    b.Navigation("Guild");
                });

            modelBuilder.Entity("Common.Db.Models.DiscordEntity", b =>
                {
                    b.HasOne("Common.Db.Models.Guild", "Guild")
                        .WithMany("DiscordEntities")
                        .HasForeignKey("GuildId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_discord_entities_guilds_guild_id");

                    b.Navigation("Guild");
                });

            modelBuilder.Entity("Common.Db.Models.Member", b =>
                {
                    b.HasOne("Common.Db.Models.Guild", "Guild")
                        .WithMany("Members")
                        .HasForeignKey("GuildId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_members_guilds_guild_id");

                    b.Navigation("Guild");
                });

            modelBuilder.Entity("Common.Db.Models.Quote", b =>
                {
                    b.HasOne("Common.Db.Models.Member", "Member")
                        .WithMany("Quotes")
                        .HasForeignKey("UserId", "GuildId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_quotes_members_member_temp_id1");

                    b.Navigation("Member");
                });

            modelBuilder.Entity("Common.Db.Models.SelfAssignMenu", b =>
                {
                    b.HasOne("Common.Db.Models.Guild", "Guild")
                        .WithMany("SelfAssignMenus")
                        .HasForeignKey("GuildId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_self_assign_menus_guilds_guild_id");

                    b.HasOne("Common.Db.Models.DiscordEntity", "RequiredRole")
                        .WithMany("SelfAssignMenus")
                        .HasForeignKey("RequiredRoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .HasConstraintName("fk_self_assign_menus_discord_entities_required_role_id");

                    b.Navigation("Guild");

                    b.Navigation("RequiredRole");
                });

            modelBuilder.Entity("Common.Db.Models.SelfAssignMenuDiscordEntityAssignment", b =>
                {
                    b.HasOne("Common.Db.Models.DiscordEntity", "DiscordEntity")
                        .WithMany("SelfAssignMenuDiscordEntityAssignments")
                        .HasForeignKey("DiscordEntityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_self_assign_menu_discord_entity_assignments_discord_entitie");

                    b.HasOne("Common.Db.Models.SelfAssignMenu", "SelfAssignMenu")
                        .WithMany("SelfAssignMenuDiscordEntityAssignments")
                        .HasForeignKey("SelfAssignMenuId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_self_assign_menu_discord_entity_assignments_self_assign_men");

                    b.Navigation("DiscordEntity");

                    b.Navigation("SelfAssignMenu");
                });

            modelBuilder.Entity("Common.Db.Models.Stash", b =>
                {
                    b.HasOne("Common.Db.Models.Guild", "Guild")
                        .WithMany("Stashes")
                        .HasForeignKey("GuildId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_stashes_guilds_guild_id");

                    b.HasOne("Common.Db.Models.DiscordEntity", "RequiredRole")
                        .WithMany("Stashes")
                        .HasForeignKey("RequiredRoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .HasConstraintName("fk_stashes_discord_entities_required_role_id");

                    b.Navigation("Guild");

                    b.Navigation("RequiredRole");
                });

            modelBuilder.Entity("Common.Db.Models.StashEntry", b =>
                {
                    b.HasOne("Common.Db.Models.Stash", "Stash")
                        .WithMany("StashEntries")
                        .HasForeignKey("StashId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_stash_entries_stashes_stash_id");

                    b.Navigation("Stash");
                });

            modelBuilder.Entity("Common.Db.Models.UserLog", b =>
                {
                    b.HasOne("Common.Db.Models.Member", "Member")
                        .WithMany("TargetUserLogs")
                        .HasForeignKey("AuthorId", "GuildId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_user_logs_members_member_user_id_member_guild_id");

                    b.HasOne("Common.Db.Models.Member", "Author")
                        .WithMany("AuthorUserLogs")
                        .HasForeignKey("MemberId", "GuildId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_user_logs_members_author_user_id_author_guild_id");

                    b.Navigation("Author");

                    b.Navigation("Member");
                });

            modelBuilder.Entity("Common.Db.Models.DiscordEntity", b =>
                {
                    b.Navigation("SelfAssignMenuDiscordEntityAssignments");

                    b.Navigation("SelfAssignMenus");

                    b.Navigation("Stashes");
                });

            modelBuilder.Entity("Common.Db.Models.Guild", b =>
                {
                    b.Navigation("Configs");

                    b.Navigation("DiscordEntities");

                    b.Navigation("Members");

                    b.Navigation("SelfAssignMenus");

                    b.Navigation("Stashes");
                });

            modelBuilder.Entity("Common.Db.Models.Member", b =>
                {
                    b.Navigation("AuthorUserLogs");

                    b.Navigation("CommandLogs");

                    b.Navigation("Quotes");

                    b.Navigation("TargetUserLogs");
                });

            modelBuilder.Entity("Common.Db.Models.SelfAssignMenu", b =>
                {
                    b.Navigation("SelfAssignMenuDiscordEntityAssignments");
                });

            modelBuilder.Entity("Common.Db.Models.Stash", b =>
                {
                    b.Navigation("StashEntries");
                });
#pragma warning restore 612, 618
        }
    }
}
