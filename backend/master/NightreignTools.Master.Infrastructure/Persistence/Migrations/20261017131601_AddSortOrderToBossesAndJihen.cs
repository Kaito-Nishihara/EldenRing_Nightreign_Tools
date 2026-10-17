using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NightreignTools.Master.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSortOrderToBossesAndJihen : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "sort_order",
                schema: "game",
                table: "jihen",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "sort_order",
                schema: "game",
                table: "bosses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "bosses",
                keyColumn: "id",
                keyValue: "caligo",
                column: "sort_order",
                value: 7);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "bosses",
                keyColumn: "id",
                keyValue: "edere",
                column: "sort_order",
                value: 2);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "bosses",
                keyColumn: "id",
                keyValue: "fulgor",
                column: "sort_order",
                value: 6);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "bosses",
                keyColumn: "id",
                keyValue: "gladius",
                column: "sort_order",
                value: 1);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "bosses",
                keyColumn: "id",
                keyValue: "gnostor",
                column: "sort_order",
                value: 3);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "bosses",
                keyColumn: "id",
                keyValue: "harmonia",
                column: "sort_order",
                value: 9);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "bosses",
                keyColumn: "id",
                keyValue: "libra",
                column: "sort_order",
                value: 5);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "bosses",
                keyColumn: "id",
                keyValue: "maris",
                column: "sort_order",
                value: 4);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "bosses",
                keyColumn: "id",
                keyValue: "nameless",
                column: "sort_order",
                value: 8);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "bosses",
                keyColumn: "id",
                keyValue: "strages",
                column: "sort_order",
                value: 10);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "jihen",
                keyColumn: "id",
                keyValue: "crater",
                column: "sort_order",
                value: 1);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "jihen",
                keyColumn: "id",
                keyValue: "great_cavern",
                column: "sort_order",
                value: 5);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "jihen",
                keyColumn: "id",
                keyValue: "mountain",
                column: "sort_order",
                value: 2);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "jihen",
                keyColumn: "id",
                keyValue: "noklateo",
                column: "sort_order",
                value: 4);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "jihen",
                keyColumn: "id",
                keyValue: "rotted_forest",
                column: "sort_order",
                value: 3);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "sort_order",
                schema: "game",
                table: "jihen");

            migrationBuilder.DropColumn(
                name: "sort_order",
                schema: "game",
                table: "bosses");
        }
    }
}
