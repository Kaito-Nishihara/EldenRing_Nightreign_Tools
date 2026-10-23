using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NightreignTools.Master.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddKindToBasePoints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterTable(
                name: "base_points",
                schema: "game",
                comment: "拠点候補地。マップ(通常か地変)ごとに持ち、大拠点、小拠点、強敵の種類を持つ(決定の記録 0018)。その場所に出る施設の種類は持たない(決定の記録 0010)。",
                oldComment: "拠点候補地。マップ(通常か地変)ごとに持つ。拠点の種類は持たない(決定の記録 0010)。");

            migrationBuilder.AddColumn<string>(
                name: "kind",
                schema: "game",
                table: "base_points",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "major",
                comment: "拠点の種類。major は大拠点、minor は小拠点、field_boss は強敵。");

            migrationBuilder.UpdateData(
                schema: "game",
                table: "base_points",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "kind", "x", "y" },
                values: new object[] { "major", 406m, 801m });

            migrationBuilder.UpdateData(
                schema: "game",
                table: "base_points",
                keyColumn: "id",
                keyValue: 2,
                columns: new[] { "kind", "x", "y" },
                values: new object[] { "major", 622m, 709m });

            migrationBuilder.UpdateData(
                schema: "game",
                table: "base_points",
                keyColumn: "id",
                keyValue: 3,
                columns: new[] { "kind", "x", "y" },
                values: new object[] { "major", 285m, 670m });

            migrationBuilder.UpdateData(
                schema: "game",
                table: "base_points",
                keyColumn: "id",
                keyValue: 4,
                columns: new[] { "kind", "x", "y" },
                values: new object[] { "major", 325m, 568m });

            migrationBuilder.UpdateData(
                schema: "game",
                table: "base_points",
                keyColumn: "id",
                keyValue: 5,
                columns: new[] { "kind", "x", "y" },
                values: new object[] { "major", 287m, 462m });

            migrationBuilder.UpdateData(
                schema: "game",
                table: "base_points",
                keyColumn: "id",
                keyValue: 6,
                columns: new[] { "kind", "x", "y" },
                values: new object[] { "major", 234m, 288m });

            migrationBuilder.UpdateData(
                schema: "game",
                table: "base_points",
                keyColumn: "id",
                keyValue: 7,
                columns: new[] { "kind", "x", "y" },
                values: new object[] { "major", 409m, 182m });

            migrationBuilder.UpdateData(
                schema: "game",
                table: "base_points",
                keyColumn: "id",
                keyValue: 8,
                columns: new[] { "kind", "x", "y" },
                values: new object[] { "major", 425m, 308m });

            migrationBuilder.UpdateData(
                schema: "game",
                table: "base_points",
                keyColumn: "id",
                keyValue: 9,
                columns: new[] { "kind", "x", "y" },
                values: new object[] { "major", 642m, 303m });

            migrationBuilder.UpdateData(
                schema: "game",
                table: "base_points",
                keyColumn: "id",
                keyValue: 10,
                columns: new[] { "kind", "x", "y" },
                values: new object[] { "major", 730m, 229m });

            migrationBuilder.UpdateData(
                schema: "game",
                table: "base_points",
                keyColumn: "id",
                keyValue: 11,
                columns: new[] { "kind", "x", "y" },
                values: new object[] { "major", 791m, 436m });

            migrationBuilder.UpdateData(
                schema: "game",
                table: "base_points",
                keyColumn: "id",
                keyValue: 12,
                columns: new[] { "kind", "x", "y" },
                values: new object[] { "major", 597m, 439m });

            migrationBuilder.UpdateData(
                schema: "game",
                table: "base_points",
                keyColumn: "id",
                keyValue: 13,
                columns: new[] { "kind", "x", "y" },
                values: new object[] { "major", 679m, 476m });

            migrationBuilder.UpdateData(
                schema: "game",
                table: "base_points",
                keyColumn: "id",
                keyValue: 14,
                columns: new[] { "kind", "x", "y" },
                values: new object[] { "major", 763m, 760m });

            migrationBuilder.UpdateData(
                schema: "game",
                table: "base_points",
                keyColumn: "id",
                keyValue: 15,
                columns: new[] { "kind", "x", "y" },
                values: new object[] { "major", 771m, 643m });

            migrationBuilder.InsertData(
                schema: "game",
                table: "base_points",
                columns: new[] { "id", "created_at", "hidden_at", "kind", "map_id", "name_ja", "x", "y" },
                values: new object[,]
                {
                    { 16, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "major", "normal", null, 644m, 608m },
                    { 17, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "minor", "normal", null, 549m, 245m },
                    { 18, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "minor", "normal", null, 805m, 361m },
                    { 19, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "minor", "normal", null, 228m, 368m },
                    { 20, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "minor", "normal", null, 709m, 381m },
                    { 21, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "minor", "normal", null, 368m, 412m },
                    { 22, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "minor", "normal", null, 216m, 572m },
                    { 23, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "minor", "normal", null, 828m, 597m },
                    { 24, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "minor", "normal", null, 563m, 655m },
                    { 25, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "minor", "normal", null, 464m, 721m },
                    { 26, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "minor", "normal", null, 208m, 727m },
                    { 27, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "minor", "normal", null, 590m, 827m },
                    { 28, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "field_boss", "normal", null, 247m, 198m },
                    { 29, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "field_boss", "normal", null, 644m, 213m },
                    { 30, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "field_boss", "normal", null, 335m, 241m },
                    { 31, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "field_boss", "normal", null, 477m, 370m },
                    { 32, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "field_boss", "normal", null, 313m, 380m },
                    { 33, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "field_boss", "normal", null, 561m, 386m },
                    { 34, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "field_boss", "normal", null, 776m, 513m },
                    { 35, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "field_boss", "normal", null, 422m, 736m },
                    { 36, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "field_boss", "normal", null, 704m, 792m },
                    { 37, new DateTime(2026, 10, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "field_boss", "normal", null, 315m, 835m }
                });

            migrationBuilder.AddCheckConstraint(
                name: "ck_base_points_kind",
                schema: "game",
                table: "base_points",
                sql: "kind IN ('major', 'minor', 'field_boss')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "ck_base_points_kind",
                schema: "game",
                table: "base_points");

            migrationBuilder.DeleteData(
                schema: "game",
                table: "base_points",
                keyColumn: "id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                schema: "game",
                table: "base_points",
                keyColumn: "id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                schema: "game",
                table: "base_points",
                keyColumn: "id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                schema: "game",
                table: "base_points",
                keyColumn: "id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                schema: "game",
                table: "base_points",
                keyColumn: "id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                schema: "game",
                table: "base_points",
                keyColumn: "id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                schema: "game",
                table: "base_points",
                keyColumn: "id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                schema: "game",
                table: "base_points",
                keyColumn: "id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                schema: "game",
                table: "base_points",
                keyColumn: "id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                schema: "game",
                table: "base_points",
                keyColumn: "id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                schema: "game",
                table: "base_points",
                keyColumn: "id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                schema: "game",
                table: "base_points",
                keyColumn: "id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                schema: "game",
                table: "base_points",
                keyColumn: "id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                schema: "game",
                table: "base_points",
                keyColumn: "id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                schema: "game",
                table: "base_points",
                keyColumn: "id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                schema: "game",
                table: "base_points",
                keyColumn: "id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                schema: "game",
                table: "base_points",
                keyColumn: "id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                schema: "game",
                table: "base_points",
                keyColumn: "id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                schema: "game",
                table: "base_points",
                keyColumn: "id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                schema: "game",
                table: "base_points",
                keyColumn: "id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                schema: "game",
                table: "base_points",
                keyColumn: "id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                schema: "game",
                table: "base_points",
                keyColumn: "id",
                keyValue: 37);

            migrationBuilder.DropColumn(
                name: "kind",
                schema: "game",
                table: "base_points");

            migrationBuilder.AlterTable(
                name: "base_points",
                schema: "game",
                comment: "拠点候補地。マップ(通常か地変)ごとに持つ。拠点の種類は持たない(決定の記録 0010)。",
                oldComment: "拠点候補地。マップ(通常か地変)ごとに持ち、大拠点、小拠点、強敵の種類を持つ(決定の記録 0018)。その場所に出る施設の種類は持たない(決定の記録 0010)。");

            migrationBuilder.UpdateData(
                schema: "game",
                table: "base_points",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "x", "y" },
                values: new object[] { 404m, 798m });

            migrationBuilder.UpdateData(
                schema: "game",
                table: "base_points",
                keyColumn: "id",
                keyValue: 2,
                columns: new[] { "x", "y" },
                values: new object[] { 615m, 698m });

            migrationBuilder.UpdateData(
                schema: "game",
                table: "base_points",
                keyColumn: "id",
                keyValue: 3,
                columns: new[] { "x", "y" },
                values: new object[] { 279m, 667m });

            migrationBuilder.UpdateData(
                schema: "game",
                table: "base_points",
                keyColumn: "id",
                keyValue: 4,
                columns: new[] { "x", "y" },
                values: new object[] { 317m, 562m });

            migrationBuilder.UpdateData(
                schema: "game",
                table: "base_points",
                keyColumn: "id",
                keyValue: 5,
                columns: new[] { "x", "y" },
                values: new object[] { 278m, 459m });

            migrationBuilder.UpdateData(
                schema: "game",
                table: "base_points",
                keyColumn: "id",
                keyValue: 6,
                columns: new[] { "x", "y" },
                values: new object[] { 225m, 272m });

            migrationBuilder.UpdateData(
                schema: "game",
                table: "base_points",
                keyColumn: "id",
                keyValue: 7,
                columns: new[] { "x", "y" },
                values: new object[] { 410m, 197m });

            migrationBuilder.UpdateData(
                schema: "game",
                table: "base_points",
                keyColumn: "id",
                keyValue: 8,
                columns: new[] { "x", "y" },
                values: new object[] { 434m, 316m });

            migrationBuilder.UpdateData(
                schema: "game",
                table: "base_points",
                keyColumn: "id",
                keyValue: 9,
                columns: new[] { "x", "y" },
                values: new object[] { 638m, 301m });

            migrationBuilder.UpdateData(
                schema: "game",
                table: "base_points",
                keyColumn: "id",
                keyValue: 10,
                columns: new[] { "x", "y" },
                values: new object[] { 742m, 232m });

            migrationBuilder.UpdateData(
                schema: "game",
                table: "base_points",
                keyColumn: "id",
                keyValue: 11,
                columns: new[] { "x", "y" },
                values: new object[] { 801m, 438m });

            migrationBuilder.UpdateData(
                schema: "game",
                table: "base_points",
                keyColumn: "id",
                keyValue: 12,
                columns: new[] { "x", "y" },
                values: new object[] { 575m, 451m });

            migrationBuilder.UpdateData(
                schema: "game",
                table: "base_points",
                keyColumn: "id",
                keyValue: 13,
                columns: new[] { "x", "y" },
                values: new object[] { 677m, 501m });

            migrationBuilder.UpdateData(
                schema: "game",
                table: "base_points",
                keyColumn: "id",
                keyValue: 14,
                columns: new[] { "x", "y" },
                values: new object[] { 766m, 763m });

            migrationBuilder.UpdateData(
                schema: "game",
                table: "base_points",
                keyColumn: "id",
                keyValue: 15,
                columns: new[] { "x", "y" },
                values: new object[] { 777m, 636m });
        }
    }
}
