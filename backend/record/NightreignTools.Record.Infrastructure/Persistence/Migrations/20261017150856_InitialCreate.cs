using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace NightreignTools.Record.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "game");

            migrationBuilder.CreateTable(
                name: "play_records",
                schema: "game",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    boss_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    jihen_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    spawn_point_id = table.Column<int>(type: "integer", nullable: false),
                    day1_night_boss_id = table.Column<int>(type: "integer", nullable: true),
                    day2_night_boss_id = table.Column<int>(type: "integer", nullable: true),
                    note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_play_records", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "shrink_entries",
                schema: "game",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    play_record_id = table.Column<int>(type: "integer", nullable: false),
                    day = table.Column<int>(type: "integer", nullable: false),
                    phase = table.Column<int>(type: "integer", nullable: false),
                    zone_id = table.Column<int>(type: "integer", nullable: true),
                    second_shrink_point_id = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_shrink_entries", x => x.id);
                    table.CheckConstraint("ck_shrink_entries_day", "day IN (1, 2)");
                    table.CheckConstraint("ck_shrink_entries_phase", "phase IN (1, 2)");
                    table.CheckConstraint("ck_shrink_entries_target", "(phase = 1 AND zone_id IS NOT NULL AND second_shrink_point_id IS NULL) OR (phase = 2 AND zone_id IS NULL AND second_shrink_point_id IS NOT NULL)");
                    table.ForeignKey(
                        name: "fk_shrink_entries_play_records_play_record_id",
                        column: x => x.play_record_id,
                        principalSchema: "game",
                        principalTable: "play_records",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_play_records_boss_id_spawn_point_id_jihen_id",
                schema: "game",
                table: "play_records",
                columns: new[] { "boss_id", "spawn_point_id", "jihen_id" });

            migrationBuilder.CreateIndex(
                name: "ix_shrink_entries_play_record_id_day_phase",
                schema: "game",
                table: "shrink_entries",
                columns: new[] { "play_record_id", "day", "phase" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "shrink_entries",
                schema: "game");

            migrationBuilder.DropTable(
                name: "play_records",
                schema: "game");
        }
    }
}
