using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace NightreignTools.Record.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTableAndColumnComments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterTable(
                name: "shrink_entries",
                schema: "game",
                comment: "1 つの記録の中の、日と段階ごとの収縮先(プレイ記録 5 章)。第 1 次(phase=1)は収縮候補地(ZoneId)だけ、第 2 次(phase=2)は第二収縮地点(SecondShrinkPointId)だけを持つ(決定の記録 0008)。マスタの行は master-service が持つので、識別子だけを持つ。");

            migrationBuilder.AlterTable(
                name: "play_records",
                schema: "game",
                comment: "1 回のプレイの記録(プレイ記録 5 章)。作った後は直さず、消さない(決定の記録 0005)。夜の王、地変、開始地点、夜ボスは master-service の行の識別子だけを持つ。あるかどうかはアプリケーションサービスが確かめる。");

            migrationBuilder.AlterColumn<int>(
                name: "zone_id",
                schema: "game",
                table: "shrink_entries",
                type: "integer",
                nullable: true,
                comment: "第 1 次の収縮先の収縮候補地の識別子。第 2 次では null。",
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "second_shrink_point_id",
                schema: "game",
                table: "shrink_entries",
                type: "integer",
                nullable: true,
                comment: "第 2 次の収縮先の第二収縮地点の識別子。第 1 次では null。",
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "play_record_id",
                schema: "game",
                table: "shrink_entries",
                type: "integer",
                nullable: false,
                comment: "この収縮先を持つ記録の識別子。",
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "phase",
                schema: "game",
                table: "shrink_entries",
                type: "integer",
                nullable: false,
                comment: "段階。1 か 2。",
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "day",
                schema: "game",
                table: "shrink_entries",
                type: "integer",
                nullable: false,
                comment: "日。1 か 2。",
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                schema: "game",
                table: "shrink_entries",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                comment: "行を作った日時(UTC)。データベースの既定値で入る。",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                schema: "game",
                table: "shrink_entries",
                type: "integer",
                nullable: false,
                comment: "識別子。",
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "spawn_point_id",
                schema: "game",
                table: "play_records",
                type: "integer",
                nullable: false,
                comment: "開始地点の識別子。master-service の spawn_points の id。",
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "note",
                schema: "game",
                table: "play_records",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                comment: "備考。前後の空白を除いたもの。空なら null。",
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "jihen_id",
                schema: "game",
                table: "play_records",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                comment: "地変の識別子。null は地変なし。",
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "day2_night_boss_id",
                schema: "game",
                table: "play_records",
                type: "integer",
                nullable: true,
                comment: "2 日目の夜ボスの識別子。null は選ばなかった。",
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "day1_night_boss_id",
                schema: "game",
                table: "play_records",
                type: "integer",
                nullable: true,
                comment: "1 日目の夜ボスの識別子。null は選ばなかった。",
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                schema: "game",
                table: "play_records",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                comment: "記録を保存した日時(UTC)。データベースの既定値で入る。",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<string>(
                name: "boss_id",
                schema: "game",
                table: "play_records",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                comment: "夜の王の識別子。master-service の bosses の id。",
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<int>(
                name: "id",
                schema: "game",
                table: "play_records",
                type: "integer",
                nullable: false,
                comment: "識別子。",
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterTable(
                name: "shrink_entries",
                schema: "game",
                oldComment: "1 つの記録の中の、日と段階ごとの収縮先(プレイ記録 5 章)。第 1 次(phase=1)は収縮候補地(ZoneId)だけ、第 2 次(phase=2)は第二収縮地点(SecondShrinkPointId)だけを持つ(決定の記録 0008)。マスタの行は master-service が持つので、識別子だけを持つ。");

            migrationBuilder.AlterTable(
                name: "play_records",
                schema: "game",
                oldComment: "1 回のプレイの記録(プレイ記録 5 章)。作った後は直さず、消さない(決定の記録 0005)。夜の王、地変、開始地点、夜ボスは master-service の行の識別子だけを持つ。あるかどうかはアプリケーションサービスが確かめる。");

            migrationBuilder.AlterColumn<int>(
                name: "zone_id",
                schema: "game",
                table: "shrink_entries",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true,
                oldComment: "第 1 次の収縮先の収縮候補地の識別子。第 2 次では null。");

            migrationBuilder.AlterColumn<int>(
                name: "second_shrink_point_id",
                schema: "game",
                table: "shrink_entries",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true,
                oldComment: "第 2 次の収縮先の第二収縮地点の識別子。第 1 次では null。");

            migrationBuilder.AlterColumn<int>(
                name: "play_record_id",
                schema: "game",
                table: "shrink_entries",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldComment: "この収縮先を持つ記録の識別子。");

            migrationBuilder.AlterColumn<int>(
                name: "phase",
                schema: "game",
                table: "shrink_entries",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldComment: "段階。1 か 2。");

            migrationBuilder.AlterColumn<int>(
                name: "day",
                schema: "game",
                table: "shrink_entries",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldComment: "日。1 か 2。");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                schema: "game",
                table: "shrink_entries",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()",
                oldComment: "行を作った日時(UTC)。データベースの既定値で入る。");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                schema: "game",
                table: "shrink_entries",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldComment: "識別子。")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "spawn_point_id",
                schema: "game",
                table: "play_records",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldComment: "開始地点の識別子。master-service の spawn_points の id。");

            migrationBuilder.AlterColumn<string>(
                name: "note",
                schema: "game",
                table: "play_records",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true,
                oldComment: "備考。前後の空白を除いたもの。空なら null。");

            migrationBuilder.AlterColumn<string>(
                name: "jihen_id",
                schema: "game",
                table: "play_records",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "地変の識別子。null は地変なし。");

            migrationBuilder.AlterColumn<int>(
                name: "day2_night_boss_id",
                schema: "game",
                table: "play_records",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true,
                oldComment: "2 日目の夜ボスの識別子。null は選ばなかった。");

            migrationBuilder.AlterColumn<int>(
                name: "day1_night_boss_id",
                schema: "game",
                table: "play_records",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true,
                oldComment: "1 日目の夜ボスの識別子。null は選ばなかった。");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                schema: "game",
                table: "play_records",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()",
                oldComment: "記録を保存した日時(UTC)。データベースの既定値で入る。");

            migrationBuilder.AlterColumn<string>(
                name: "boss_id",
                schema: "game",
                table: "play_records",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldComment: "夜の王の識別子。master-service の bosses の id。");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                schema: "game",
                table: "play_records",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldComment: "識別子。")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);
        }
    }
}
