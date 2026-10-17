using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NightreignTools.Master.Infrastructure.Persistence.Migrations
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
                name: "base_points",
                schema: "game",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:IdentitySequenceOptions", "'1001', '1', '', '', 'False', '1'")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    map_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "normal"),
                    name_ja = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    x = table.Column<decimal>(type: "numeric(7,2)", precision: 7, scale: 2, nullable: false),
                    y = table.Column<decimal>(type: "numeric(7,2)", precision: 7, scale: 2, nullable: false),
                    hidden_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_base_points", x => x.id);
                    table.CheckConstraint("ck_base_points_x", "x BETWEEN 0 AND 1024");
                    table.CheckConstraint("ck_base_points_y", "y BETWEEN 0 AND 1024");
                });

            migrationBuilder.CreateTable(
                name: "bosses",
                schema: "game",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name_ja = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    name_en = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    image_path = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    is_dlc = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    unlock_condition = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_bosses", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "jihen",
                schema: "game",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name_ja = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    image_path = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    area = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    center_x = table.Column<decimal>(type: "numeric(7,2)", precision: 7, scale: 2, nullable: false),
                    center_y = table.Column<decimal>(type: "numeric(7,2)", precision: 7, scale: 2, nullable: false),
                    is_dlc = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    unlock_condition = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_jihen", x => x.id);
                    table.CheckConstraint("ck_jihen_center_x", "center_x BETWEEN 0 AND 1024");
                    table.CheckConstraint("ck_jihen_center_y", "center_y BETWEEN 0 AND 1024");
                });

            migrationBuilder.CreateTable(
                name: "night_bosses",
                schema: "game",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:IdentitySequenceOptions", "'1001', '1', '', '', 'False', '1'")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name_ja = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    day = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_night_bosses", x => x.id);
                    table.CheckConstraint("ck_night_bosses_day", "day IN (1, 2)");
                });

            migrationBuilder.CreateTable(
                name: "second_shrink_points",
                schema: "game",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:IdentitySequenceOptions", "'1001', '1', '', '', 'False', '1'")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name_ja = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    x = table.Column<decimal>(type: "numeric(7,2)", precision: 7, scale: 2, nullable: false),
                    y = table.Column<decimal>(type: "numeric(7,2)", precision: 7, scale: 2, nullable: false),
                    hidden_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_second_shrink_points", x => x.id);
                    table.CheckConstraint("ck_second_shrink_points_x", "x BETWEEN 0 AND 1024");
                    table.CheckConstraint("ck_second_shrink_points_y", "y BETWEEN 0 AND 1024");
                });

            migrationBuilder.CreateTable(
                name: "shrink_zones",
                schema: "game",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:IdentitySequenceOptions", "'1001', '1', '', '', 'False', '1'")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name_ja = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    icon_path = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    center_x = table.Column<decimal>(type: "numeric(7,2)", precision: 7, scale: 2, nullable: false),
                    center_y = table.Column<decimal>(type: "numeric(7,2)", precision: 7, scale: 2, nullable: false),
                    radius = table.Column<decimal>(type: "numeric(7,2)", precision: 7, scale: 2, nullable: false),
                    hidden_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_shrink_zones", x => x.id);
                    table.CheckConstraint("ck_shrink_zones_center_x", "center_x BETWEEN 0 AND 1024");
                    table.CheckConstraint("ck_shrink_zones_center_y", "center_y BETWEEN 0 AND 1024");
                    table.CheckConstraint("ck_shrink_zones_radius", "radius BETWEEN 0 AND 1024");
                });

            migrationBuilder.CreateTable(
                name: "spawn_points",
                schema: "game",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:IdentitySequenceOptions", "'1001', '1', '', '', 'False', '1'")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name_ja = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    x = table.Column<decimal>(type: "numeric(7,2)", precision: 7, scale: 2, nullable: false),
                    y = table.Column<decimal>(type: "numeric(7,2)", precision: 7, scale: 2, nullable: false),
                    hidden_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_spawn_points", x => x.id);
                    table.CheckConstraint("ck_spawn_points_x", "x BETWEEN 0 AND 1024");
                    table.CheckConstraint("ck_spawn_points_y", "y BETWEEN 0 AND 1024");
                });

            migrationBuilder.CreateTable(
                name: "boss_weaknesses",
                schema: "game",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:IdentitySequenceOptions", "'1001', '1', '', '', 'False', '1'")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    boss_id = table.Column<string>(type: "character varying(50)", nullable: false),
                    attribute = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_boss_weaknesses", x => x.id);
                    table.ForeignKey(
                        name: "fk_boss_weaknesses_bosses_boss_id",
                        column: x => x.boss_id,
                        principalSchema: "game",
                        principalTable: "bosses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "game",
                table: "base_points",
                columns: new[] { "id", "created_at", "hidden_at", "map_id", "name_ja", "x", "y" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "normal", null, 404m, 798m },
                    { 2, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "normal", null, 615m, 698m },
                    { 3, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "normal", null, 279m, 667m },
                    { 4, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "normal", null, 317m, 562m },
                    { 5, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "normal", null, 278m, 459m },
                    { 6, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "normal", null, 225m, 272m },
                    { 7, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "normal", null, 410m, 197m },
                    { 8, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "normal", null, 434m, 316m },
                    { 9, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "normal", null, 638m, 301m },
                    { 10, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "normal", null, 742m, 232m },
                    { 11, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "normal", null, 801m, 438m },
                    { 12, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "normal", null, 575m, 451m },
                    { 13, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "normal", null, 677m, 501m },
                    { 14, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "normal", null, 766m, 763m },
                    { 15, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "normal", null, 777m, 636m }
                });

            migrationBuilder.InsertData(
                schema: "game",
                table: "bosses",
                columns: new[] { "id", "created_at", "image_path", "name_en", "name_ja", "unlock_condition" },
                values: new object[,]
                {
                    { "caligo", new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "Caligo", "霧の裂け目", null },
                    { "edere", new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "Edere", "喰らいつく顎", null },
                    { "fulgor", new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "Fulgor", "闇駆ける狩人", null },
                    { "gladius", new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "Gladius", "三つ首の獣", null },
                    { "gnostor", new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "Gnostor", "知性の蟲", null }
                });

            migrationBuilder.InsertData(
                schema: "game",
                table: "bosses",
                columns: new[] { "id", "created_at", "image_path", "is_dlc", "name_en", "name_ja", "unlock_condition" },
                values: new object[] { "harmonia", new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, true, "Harmonia", "英雄武器の娘たち", null });

            migrationBuilder.InsertData(
                schema: "game",
                table: "bosses",
                columns: new[] { "id", "created_at", "image_path", "name_en", "name_ja", "unlock_condition" },
                values: new object[,]
                {
                    { "libra", new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "Libra", "調律の魔物", null },
                    { "maris", new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "Maris", "兆し", null },
                    { "nameless", new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "Nameless", "夜を象る者", "4体撃破後" }
                });

            migrationBuilder.InsertData(
                schema: "game",
                table: "bosses",
                columns: new[] { "id", "created_at", "image_path", "is_dlc", "name_en", "name_ja", "unlock_condition" },
                values: new object[] { "strages", new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, true, "Strages", "反逆のストラゲス", null });

            migrationBuilder.InsertData(
                schema: "game",
                table: "jihen",
                columns: new[] { "id", "area", "center_x", "center_y", "created_at", "image_path", "name_ja", "unlock_condition" },
                values: new object[] { "crater", "north", 512m, 204.8m, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "火口", null });

            migrationBuilder.InsertData(
                schema: "game",
                table: "jihen",
                columns: new[] { "id", "area", "center_x", "center_y", "created_at", "image_path", "is_dlc", "name_ja", "unlock_condition" },
                values: new object[] { "great_cavern", "all", 512m, 512m, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, true, "大空洞", null });

            migrationBuilder.InsertData(
                schema: "game",
                table: "jihen",
                columns: new[] { "id", "area", "center_x", "center_y", "created_at", "image_path", "name_ja", "unlock_condition" },
                values: new object[,]
                {
                    { "mountain", "northwest", 256m, 256m, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "山嶺", null },
                    { "noklateo", "center", 512m, 512m, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "隠れ都ノクラテオ", null },
                    { "rotted_forest", "southwest", 256m, 768m, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "腐れ森", null }
                });

            migrationBuilder.InsertData(
                schema: "game",
                table: "night_bosses",
                columns: new[] { "id", "created_at", "day", "name_ja" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), 1, "亜人の女王＆亜人の剣聖" },
                    { 2, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), 1, "鈴玉狩り" },
                    { 3, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), 1, "貪食ドラゴン" },
                    { 4, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), 1, "夜の騎兵：グレイブ＆夜の騎兵：フレイル" },
                    { 5, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), 1, "英雄のガーゴイル" },
                    { 6, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), 1, "ミミズ顔" },
                    { 7, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), 1, "百足のデーモン" },
                    { 8, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), 1, "熔鉄デーモン" },
                    { 9, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), 1, "戦場の宿将" },
                    { 10, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), 1, "爛れた樹霊" },
                    { 11, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), 1, "接ぎ木の君主" },
                    { 12, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), 1, "ティビアの呼び舟＆死に生きる者たち" },
                    { 13, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), 1, "王族の幽鬼" },
                    { 14, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), 1, "公のフレイディア" },
                    { 15, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), 2, "ツリーガード＆王都の騎兵" },
                    { 16, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), 2, "忌み鬼" },
                    { 17, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), 2, "古竜" },
                    { 18, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), 2, "坩堝の騎士＆黄金カバ" },
                    { 19, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), 2, "僻地の宿将" },
                    { 20, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), 2, "ノクスの竜人兵" },
                    { 21, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), 2, "竜のツリーガード＆王都の騎兵" },
                    { 22, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), 2, "大土竜" },
                    { 23, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), 2, "神肌の貴種＆神肌の使徒" },
                    { 24, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), 2, "降る星の成獣" },
                    { 25, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), 2, "冷たい谷の踊り子" },
                    { 26, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), 2, "無名の王" },
                    { 27, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), 2, "死儀礼の鳥" },
                    { 28, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), 2, "傷ついたデーモン＆うろ底のデーモン" },
                    { 29, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), 2, "デーモンの王子" },
                    { 30, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), 2, "呪剣士＆神獣の戦士" },
                    { 31, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), 2, "血の君主" },
                    { 32, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), 2, "大赤熊" },
                    { 33, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), 2, "神獣獅子舞" },
                    { 34, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), 2, "死の騎士" },
                    { 35, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), 2, "騎士アルトリウス" }
                });

            migrationBuilder.InsertData(
                schema: "game",
                table: "second_shrink_points",
                columns: new[] { "id", "created_at", "hidden_at", "name_ja", "x", "y" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "最北西", 219m, 233m },
                    { 2, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "中央西", 342m, 488m },
                    { 3, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "最北東", 789m, 248m },
                    { 4, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "最北", 583m, 297m },
                    { 5, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "最東", 739m, 518m },
                    { 6, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "中央東", 558m, 552m },
                    { 7, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "最南東", 715m, 692m },
                    { 8, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "最南", 493m, 774m },
                    { 9, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "中央南西", 383m, 711m },
                    { 10, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "最南西", 277m, 728m },
                    { 11, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "最西", 211m, 402m },
                    { 12, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "中央南", 408m, 624m }
                });

            migrationBuilder.InsertData(
                schema: "game",
                table: "shrink_zones",
                columns: new[] { "id", "center_x", "center_y", "created_at", "hidden_at", "icon_path", "name_ja", "radius" },
                values: new object[,]
                {
                    { 1, 219m, 233m, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "最北西", 255m },
                    { 2, 342m, 488m, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "中央西", 255m },
                    { 3, 789m, 248m, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "最北東", 255m },
                    { 4, 583m, 297m, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "最北", 255m },
                    { 5, 739m, 518m, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "最東", 255m },
                    { 6, 558m, 552m, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "中央東", 255m },
                    { 7, 715m, 692m, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "最南東", 255m },
                    { 8, 493m, 774m, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "最南", 255m },
                    { 9, 383m, 711m, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "中央南西", 255m },
                    { 10, 277m, 728m, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "最南西", 255m },
                    { 11, 211m, 402m, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "最西", 255m },
                    { 12, 408m, 624m, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "中央南", 255m }
                });

            migrationBuilder.InsertData(
                schema: "game",
                table: "spawn_points",
                columns: new[] { "id", "created_at", "hidden_at", "name_ja", "x", "y" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "北東の教会", 351m, 387m },
                    { 2, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "西の教会", 216m, 578m },
                    { 3, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "南西の教会", 218m, 763m },
                    { 4, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "南の教会", 577m, 830m },
                    { 5, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "南東の教会", 561m, 679m },
                    { 6, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "北の教会", 547m, 215m },
                    { 7, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "北西の教会", 218m, 360m },
                    { 8, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "東の教会", 697m, 398m },
                    { 9, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "東の野営地", 808m, 569m }
                });

            migrationBuilder.InsertData(
                schema: "game",
                table: "boss_weaknesses",
                columns: new[] { "id", "attribute", "boss_id", "created_at" },
                values: new object[,]
                {
                    { 1, "holy", "gladius", new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, "poison", "edere", new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, "fire", "gnostor", new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4, "lightning", "maris", new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 5, "holy", "libra", new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 6, "fire", "libra", new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 7, "madness", "libra", new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 8, "lightning", "fulgor", new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 9, "fire", "caligo", new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 10, "holy", "nameless", new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 11, "fire", "nameless", new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 12, "lightning", "nameless", new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 13, "sleep", "harmonia", new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 14, "holy", "strages", new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 15, "fire", "strages", new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.CreateIndex(
                name: "ix_base_points_map_id",
                schema: "game",
                table: "base_points",
                column: "map_id");

            migrationBuilder.CreateIndex(
                name: "ix_boss_weaknesses_boss_id",
                schema: "game",
                table: "boss_weaknesses",
                column: "boss_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "base_points",
                schema: "game");

            migrationBuilder.DropTable(
                name: "boss_weaknesses",
                schema: "game");

            migrationBuilder.DropTable(
                name: "jihen",
                schema: "game");

            migrationBuilder.DropTable(
                name: "night_bosses",
                schema: "game");

            migrationBuilder.DropTable(
                name: "second_shrink_points",
                schema: "game");

            migrationBuilder.DropTable(
                name: "shrink_zones",
                schema: "game");

            migrationBuilder.DropTable(
                name: "spawn_points",
                schema: "game");

            migrationBuilder.DropTable(
                name: "bosses",
                schema: "game");
        }
    }
}
