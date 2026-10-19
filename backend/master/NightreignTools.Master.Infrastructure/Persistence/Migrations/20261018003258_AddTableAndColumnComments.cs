using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace NightreignTools.Master.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTableAndColumnComments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterTable(
                name: "spawn_points",
                schema: "game",
                comment: "開始地点。プレイの始まりの場所。");

            migrationBuilder.AlterTable(
                name: "shrink_zones",
                schema: "game",
                comment: "収縮候補地。第 1 次収縮の円の中心と半径。");

            migrationBuilder.AlterTable(
                name: "second_shrink_points",
                schema: "game",
                comment: "第二収縮地点。第 2 次収縮の向かう点(決定の記録 0008)。");

            migrationBuilder.AlterTable(
                name: "night_bosses",
                schema: "game",
                comment: "1 日目か 2 日目の夜に出る夜ボス。夜の王とは別に持つ。");

            migrationBuilder.AlterTable(
                name: "jihen",
                schema: "game",
                comment: "地変。地変ごとにマップ画像が替わる。中心の座標は 1024 四方の座標空間の数。");

            migrationBuilder.AlterTable(
                name: "bosses",
                schema: "game",
                comment: "夜の王(3 日目に戦う相手)。識別子は英小文字の固定の値。マイグレーションの初期データだけで管理する。");

            migrationBuilder.AlterTable(
                name: "boss_weaknesses",
                schema: "game",
                comment: "夜の王の弱点の属性。例: holy、fire。");

            migrationBuilder.AlterTable(
                name: "base_points",
                schema: "game",
                comment: "拠点候補地。マップ(通常か地変)ごとに持つ。拠点の種類は持たない(決定の記録 0010)。");

            migrationBuilder.AlterColumn<decimal>(
                name: "y",
                schema: "game",
                table: "spawn_points",
                type: "numeric(7,2)",
                precision: 7,
                scale: 2,
                nullable: false,
                comment: "y 座標。1024 四方の座標空間の数。",
                oldClrType: typeof(decimal),
                oldType: "numeric(7,2)",
                oldPrecision: 7,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "x",
                schema: "game",
                table: "spawn_points",
                type: "numeric(7,2)",
                precision: 7,
                scale: 2,
                nullable: false,
                comment: "x 座標。1024 四方の座標空間の数。",
                oldClrType: typeof(decimal),
                oldType: "numeric(7,2)",
                oldPrecision: 7,
                oldScale: 2);

            migrationBuilder.AlterColumn<string>(
                name: "name_ja",
                schema: "game",
                table: "spawn_points",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                comment: "日本語の名前。",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<DateTime>(
                name: "hidden_at",
                schema: "game",
                table: "spawn_points",
                type: "timestamp with time zone",
                nullable: true,
                comment: "非表示にした日時。null は表示中。",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                schema: "game",
                table: "spawn_points",
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
                table: "spawn_points",
                type: "integer",
                nullable: false,
                comment: "識別子。初期データは 1000 以下、管理の画面で足した行は 1001 から。",
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:IdentitySequenceOptions", "'1001', '1', '', '', 'False', '1'")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:IdentitySequenceOptions", "'1001', '1', '', '', 'False', '1'")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<decimal>(
                name: "radius",
                schema: "game",
                table: "shrink_zones",
                type: "numeric(7,2)",
                precision: 7,
                scale: 2,
                nullable: false,
                comment: "円の半径。1024 四方の座標空間の数。",
                oldClrType: typeof(decimal),
                oldType: "numeric(7,2)",
                oldPrecision: 7,
                oldScale: 2);

            migrationBuilder.AlterColumn<string>(
                name: "name_ja",
                schema: "game",
                table: "shrink_zones",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                comment: "日本語の名前。",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "icon_path",
                schema: "game",
                table: "shrink_zones",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                comment: "アイコンの画像のパス。null はアイコンなし。",
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "hidden_at",
                schema: "game",
                table: "shrink_zones",
                type: "timestamp with time zone",
                nullable: true,
                comment: "非表示にした日時。null は表示中。",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                schema: "game",
                table: "shrink_zones",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                comment: "行を作った日時(UTC)。データベースの既定値で入る。",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<decimal>(
                name: "center_y",
                schema: "game",
                table: "shrink_zones",
                type: "numeric(7,2)",
                precision: 7,
                scale: 2,
                nullable: false,
                comment: "円の中心の y 座標。1024 四方の座標空間の数。",
                oldClrType: typeof(decimal),
                oldType: "numeric(7,2)",
                oldPrecision: 7,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "center_x",
                schema: "game",
                table: "shrink_zones",
                type: "numeric(7,2)",
                precision: 7,
                scale: 2,
                nullable: false,
                comment: "円の中心の x 座標。1024 四方の座標空間の数。",
                oldClrType: typeof(decimal),
                oldType: "numeric(7,2)",
                oldPrecision: 7,
                oldScale: 2);

            migrationBuilder.AlterColumn<int>(
                name: "id",
                schema: "game",
                table: "shrink_zones",
                type: "integer",
                nullable: false,
                comment: "識別子。初期データは 1000 以下、管理の画面で足した行は 1001 から。",
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:IdentitySequenceOptions", "'1001', '1', '', '', 'False', '1'")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:IdentitySequenceOptions", "'1001', '1', '', '', 'False', '1'")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<decimal>(
                name: "y",
                schema: "game",
                table: "second_shrink_points",
                type: "numeric(7,2)",
                precision: 7,
                scale: 2,
                nullable: false,
                comment: "y 座標。1024 四方の座標空間の数。",
                oldClrType: typeof(decimal),
                oldType: "numeric(7,2)",
                oldPrecision: 7,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "x",
                schema: "game",
                table: "second_shrink_points",
                type: "numeric(7,2)",
                precision: 7,
                scale: 2,
                nullable: false,
                comment: "x 座標。1024 四方の座標空間の数。",
                oldClrType: typeof(decimal),
                oldType: "numeric(7,2)",
                oldPrecision: 7,
                oldScale: 2);

            migrationBuilder.AlterColumn<string>(
                name: "name_ja",
                schema: "game",
                table: "second_shrink_points",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                comment: "日本語の名前。",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<DateTime>(
                name: "hidden_at",
                schema: "game",
                table: "second_shrink_points",
                type: "timestamp with time zone",
                nullable: true,
                comment: "非表示にした日時。null は表示中。",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                schema: "game",
                table: "second_shrink_points",
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
                table: "second_shrink_points",
                type: "integer",
                nullable: false,
                comment: "識別子。初期データは 1000 以下、管理の画面で足した行は 1001 から。",
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:IdentitySequenceOptions", "'1001', '1', '', '', 'False', '1'")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:IdentitySequenceOptions", "'1001', '1', '', '', 'False', '1'")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<string>(
                name: "name_ja",
                schema: "game",
                table: "night_bosses",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                comment: "日本語の名前。",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<int>(
                name: "day",
                schema: "game",
                table: "night_bosses",
                type: "integer",
                nullable: false,
                comment: "出る日。1 か 2。",
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                schema: "game",
                table: "night_bosses",
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
                table: "night_bosses",
                type: "integer",
                nullable: false,
                comment: "識別子。",
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:IdentitySequenceOptions", "'1001', '1', '', '', 'False', '1'")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:IdentitySequenceOptions", "'1001', '1', '', '', 'False', '1'")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<string>(
                name: "unlock_condition",
                schema: "game",
                table: "jihen",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                comment: "起きるようになる条件。null は最初から起きる。",
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "sort_order",
                schema: "game",
                table: "jihen",
                type: "integer",
                nullable: false,
                comment: "画面の選択の欄での並び順。1 から(データベースの共通仕様 5.6)。",
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "name_ja",
                schema: "game",
                table: "jihen",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                comment: "日本語の名前。",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<bool>(
                name: "is_dlc",
                schema: "game",
                table: "jihen",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                comment: "ダウンロードコンテンツで足された地変か。",
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "image_path",
                schema: "game",
                table: "jihen",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                comment: "地変のマップ画像のパス。null は画像なし。",
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                schema: "game",
                table: "jihen",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                comment: "行を作った日時(UTC)。データベースの既定値で入る。",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<decimal>(
                name: "center_y",
                schema: "game",
                table: "jihen",
                type: "numeric(7,2)",
                precision: 7,
                scale: 2,
                nullable: false,
                comment: "地変の中心の y 座標。1024 四方の座標空間の数。",
                oldClrType: typeof(decimal),
                oldType: "numeric(7,2)",
                oldPrecision: 7,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "center_x",
                schema: "game",
                table: "jihen",
                type: "numeric(7,2)",
                precision: 7,
                scale: 2,
                nullable: false,
                comment: "地変の中心の x 座標。1024 四方の座標空間の数。",
                oldClrType: typeof(decimal),
                oldType: "numeric(7,2)",
                oldPrecision: 7,
                oldScale: 2);

            migrationBuilder.AlterColumn<string>(
                name: "area",
                schema: "game",
                table: "jihen",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                comment: "地変が起きる方角。例: north、center、all。",
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "id",
                schema: "game",
                table: "jihen",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                comment: "識別子。英小文字の固定の値。例: crater。",
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "unlock_condition",
                schema: "game",
                table: "bosses",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                comment: "戦えるようになる条件。null は最初から戦える。",
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "sort_order",
                schema: "game",
                table: "bosses",
                type: "integer",
                nullable: false,
                comment: "画面の選択の欄での並び順。1 から(データベースの共通仕様 5.6)。",
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "name_ja",
                schema: "game",
                table: "bosses",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                comment: "日本語の名前。",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "name_en",
                schema: "game",
                table: "bosses",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                comment: "英語の名前。",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "is_dlc",
                schema: "game",
                table: "bosses",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                comment: "ダウンロードコンテンツで足された夜の王か。",
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "image_path",
                schema: "game",
                table: "bosses",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                comment: "画像のパス。null は画像なし。",
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                schema: "game",
                table: "bosses",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                comment: "行を作った日時(UTC)。データベースの既定値で入る。",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<string>(
                name: "id",
                schema: "game",
                table: "bosses",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                comment: "識別子。英小文字の固定の値。例: gladius。",
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                schema: "game",
                table: "boss_weaknesses",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                comment: "行を作った日時(UTC)。データベースの既定値で入る。",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<string>(
                name: "boss_id",
                schema: "game",
                table: "boss_weaknesses",
                type: "character varying(50)",
                nullable: false,
                comment: "夜の王の識別子。",
                oldClrType: typeof(string),
                oldType: "character varying(50)");

            migrationBuilder.AlterColumn<string>(
                name: "attribute",
                schema: "game",
                table: "boss_weaknesses",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                comment: "弱点の属性。例: holy、fire。",
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<int>(
                name: "id",
                schema: "game",
                table: "boss_weaknesses",
                type: "integer",
                nullable: false,
                comment: "識別子。",
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:IdentitySequenceOptions", "'1001', '1', '', '', 'False', '1'")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:IdentitySequenceOptions", "'1001', '1', '', '', 'False', '1'")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<decimal>(
                name: "y",
                schema: "game",
                table: "base_points",
                type: "numeric(7,2)",
                precision: 7,
                scale: 2,
                nullable: false,
                comment: "y 座標。1024 四方の座標空間の数。",
                oldClrType: typeof(decimal),
                oldType: "numeric(7,2)",
                oldPrecision: 7,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "x",
                schema: "game",
                table: "base_points",
                type: "numeric(7,2)",
                precision: 7,
                scale: 2,
                nullable: false,
                comment: "x 座標。1024 四方の座標空間の数。",
                oldClrType: typeof(decimal),
                oldType: "numeric(7,2)",
                oldPrecision: 7,
                oldScale: 2);

            migrationBuilder.AlterColumn<string>(
                name: "name_ja",
                schema: "game",
                table: "base_points",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                comment: "日本語の名前。null は名前なし。",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "map_id",
                schema: "game",
                table: "base_points",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "normal",
                comment: "どのマップの拠点か。normal は通常のマップ、ほかは地変の識別子。",
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldDefaultValue: "normal");

            migrationBuilder.AlterColumn<DateTime>(
                name: "hidden_at",
                schema: "game",
                table: "base_points",
                type: "timestamp with time zone",
                nullable: true,
                comment: "非表示にした日時。null は表示中。",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                schema: "game",
                table: "base_points",
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
                table: "base_points",
                type: "integer",
                nullable: false,
                comment: "識別子。初期データは 1000 以下、管理の画面で足した行は 1001 から。",
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:IdentitySequenceOptions", "'1001', '1', '', '', 'False', '1'")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:IdentitySequenceOptions", "'1001', '1', '', '', 'False', '1'")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterTable(
                name: "spawn_points",
                schema: "game",
                oldComment: "開始地点。プレイの始まりの場所。");

            migrationBuilder.AlterTable(
                name: "shrink_zones",
                schema: "game",
                oldComment: "収縮候補地。第 1 次収縮の円の中心と半径。");

            migrationBuilder.AlterTable(
                name: "second_shrink_points",
                schema: "game",
                oldComment: "第二収縮地点。第 2 次収縮の向かう点(決定の記録 0008)。");

            migrationBuilder.AlterTable(
                name: "night_bosses",
                schema: "game",
                oldComment: "1 日目か 2 日目の夜に出る夜ボス。夜の王とは別に持つ。");

            migrationBuilder.AlterTable(
                name: "jihen",
                schema: "game",
                oldComment: "地変。地変ごとにマップ画像が替わる。中心の座標は 1024 四方の座標空間の数。");

            migrationBuilder.AlterTable(
                name: "bosses",
                schema: "game",
                oldComment: "夜の王(3 日目に戦う相手)。識別子は英小文字の固定の値。マイグレーションの初期データだけで管理する。");

            migrationBuilder.AlterTable(
                name: "boss_weaknesses",
                schema: "game",
                oldComment: "夜の王の弱点の属性。例: holy、fire。");

            migrationBuilder.AlterTable(
                name: "base_points",
                schema: "game",
                oldComment: "拠点候補地。マップ(通常か地変)ごとに持つ。拠点の種類は持たない(決定の記録 0010)。");

            migrationBuilder.AlterColumn<decimal>(
                name: "y",
                schema: "game",
                table: "spawn_points",
                type: "numeric(7,2)",
                precision: 7,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(7,2)",
                oldPrecision: 7,
                oldScale: 2,
                oldComment: "y 座標。1024 四方の座標空間の数。");

            migrationBuilder.AlterColumn<decimal>(
                name: "x",
                schema: "game",
                table: "spawn_points",
                type: "numeric(7,2)",
                precision: 7,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(7,2)",
                oldPrecision: 7,
                oldScale: 2,
                oldComment: "x 座標。1024 四方の座標空間の数。");

            migrationBuilder.AlterColumn<string>(
                name: "name_ja",
                schema: "game",
                table: "spawn_points",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldComment: "日本語の名前。");

            migrationBuilder.AlterColumn<DateTime>(
                name: "hidden_at",
                schema: "game",
                table: "spawn_points",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true,
                oldComment: "非表示にした日時。null は表示中。");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                schema: "game",
                table: "spawn_points",
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
                table: "spawn_points",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldComment: "識別子。初期データは 1000 以下、管理の画面で足した行は 1001 から。")
                .Annotation("Npgsql:IdentitySequenceOptions", "'1001', '1', '', '', 'False', '1'")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:IdentitySequenceOptions", "'1001', '1', '', '', 'False', '1'")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<decimal>(
                name: "radius",
                schema: "game",
                table: "shrink_zones",
                type: "numeric(7,2)",
                precision: 7,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(7,2)",
                oldPrecision: 7,
                oldScale: 2,
                oldComment: "円の半径。1024 四方の座標空間の数。");

            migrationBuilder.AlterColumn<string>(
                name: "name_ja",
                schema: "game",
                table: "shrink_zones",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldComment: "日本語の名前。");

            migrationBuilder.AlterColumn<string>(
                name: "icon_path",
                schema: "game",
                table: "shrink_zones",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldComment: "アイコンの画像のパス。null はアイコンなし。");

            migrationBuilder.AlterColumn<DateTime>(
                name: "hidden_at",
                schema: "game",
                table: "shrink_zones",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true,
                oldComment: "非表示にした日時。null は表示中。");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                schema: "game",
                table: "shrink_zones",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()",
                oldComment: "行を作った日時(UTC)。データベースの既定値で入る。");

            migrationBuilder.AlterColumn<decimal>(
                name: "center_y",
                schema: "game",
                table: "shrink_zones",
                type: "numeric(7,2)",
                precision: 7,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(7,2)",
                oldPrecision: 7,
                oldScale: 2,
                oldComment: "円の中心の y 座標。1024 四方の座標空間の数。");

            migrationBuilder.AlterColumn<decimal>(
                name: "center_x",
                schema: "game",
                table: "shrink_zones",
                type: "numeric(7,2)",
                precision: 7,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(7,2)",
                oldPrecision: 7,
                oldScale: 2,
                oldComment: "円の中心の x 座標。1024 四方の座標空間の数。");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                schema: "game",
                table: "shrink_zones",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldComment: "識別子。初期データは 1000 以下、管理の画面で足した行は 1001 から。")
                .Annotation("Npgsql:IdentitySequenceOptions", "'1001', '1', '', '', 'False', '1'")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:IdentitySequenceOptions", "'1001', '1', '', '', 'False', '1'")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<decimal>(
                name: "y",
                schema: "game",
                table: "second_shrink_points",
                type: "numeric(7,2)",
                precision: 7,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(7,2)",
                oldPrecision: 7,
                oldScale: 2,
                oldComment: "y 座標。1024 四方の座標空間の数。");

            migrationBuilder.AlterColumn<decimal>(
                name: "x",
                schema: "game",
                table: "second_shrink_points",
                type: "numeric(7,2)",
                precision: 7,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(7,2)",
                oldPrecision: 7,
                oldScale: 2,
                oldComment: "x 座標。1024 四方の座標空間の数。");

            migrationBuilder.AlterColumn<string>(
                name: "name_ja",
                schema: "game",
                table: "second_shrink_points",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldComment: "日本語の名前。");

            migrationBuilder.AlterColumn<DateTime>(
                name: "hidden_at",
                schema: "game",
                table: "second_shrink_points",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true,
                oldComment: "非表示にした日時。null は表示中。");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                schema: "game",
                table: "second_shrink_points",
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
                table: "second_shrink_points",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldComment: "識別子。初期データは 1000 以下、管理の画面で足した行は 1001 から。")
                .Annotation("Npgsql:IdentitySequenceOptions", "'1001', '1', '', '', 'False', '1'")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:IdentitySequenceOptions", "'1001', '1', '', '', 'False', '1'")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<string>(
                name: "name_ja",
                schema: "game",
                table: "night_bosses",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldComment: "日本語の名前。");

            migrationBuilder.AlterColumn<int>(
                name: "day",
                schema: "game",
                table: "night_bosses",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldComment: "出る日。1 か 2。");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                schema: "game",
                table: "night_bosses",
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
                table: "night_bosses",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldComment: "識別子。")
                .Annotation("Npgsql:IdentitySequenceOptions", "'1001', '1', '', '', 'False', '1'")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:IdentitySequenceOptions", "'1001', '1', '', '', 'False', '1'")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<string>(
                name: "unlock_condition",
                schema: "game",
                table: "jihen",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldComment: "起きるようになる条件。null は最初から起きる。");

            migrationBuilder.AlterColumn<int>(
                name: "sort_order",
                schema: "game",
                table: "jihen",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldComment: "画面の選択の欄での並び順。1 から(データベースの共通仕様 5.6)。");

            migrationBuilder.AlterColumn<string>(
                name: "name_ja",
                schema: "game",
                table: "jihen",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldComment: "日本語の名前。");

            migrationBuilder.AlterColumn<bool>(
                name: "is_dlc",
                schema: "game",
                table: "jihen",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false,
                oldComment: "ダウンロードコンテンツで足された地変か。");

            migrationBuilder.AlterColumn<string>(
                name: "image_path",
                schema: "game",
                table: "jihen",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldComment: "地変のマップ画像のパス。null は画像なし。");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                schema: "game",
                table: "jihen",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()",
                oldComment: "行を作った日時(UTC)。データベースの既定値で入る。");

            migrationBuilder.AlterColumn<decimal>(
                name: "center_y",
                schema: "game",
                table: "jihen",
                type: "numeric(7,2)",
                precision: 7,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(7,2)",
                oldPrecision: 7,
                oldScale: 2,
                oldComment: "地変の中心の y 座標。1024 四方の座標空間の数。");

            migrationBuilder.AlterColumn<decimal>(
                name: "center_x",
                schema: "game",
                table: "jihen",
                type: "numeric(7,2)",
                precision: 7,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(7,2)",
                oldPrecision: 7,
                oldScale: 2,
                oldComment: "地変の中心の x 座標。1024 四方の座標空間の数。");

            migrationBuilder.AlterColumn<string>(
                name: "area",
                schema: "game",
                table: "jihen",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldComment: "地変が起きる方角。例: north、center、all。");

            migrationBuilder.AlterColumn<string>(
                name: "id",
                schema: "game",
                table: "jihen",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldComment: "識別子。英小文字の固定の値。例: crater。");

            migrationBuilder.AlterColumn<string>(
                name: "unlock_condition",
                schema: "game",
                table: "bosses",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldComment: "戦えるようになる条件。null は最初から戦える。");

            migrationBuilder.AlterColumn<int>(
                name: "sort_order",
                schema: "game",
                table: "bosses",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldComment: "画面の選択の欄での並び順。1 から(データベースの共通仕様 5.6)。");

            migrationBuilder.AlterColumn<string>(
                name: "name_ja",
                schema: "game",
                table: "bosses",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldComment: "日本語の名前。");

            migrationBuilder.AlterColumn<string>(
                name: "name_en",
                schema: "game",
                table: "bosses",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "英語の名前。");

            migrationBuilder.AlterColumn<bool>(
                name: "is_dlc",
                schema: "game",
                table: "bosses",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false,
                oldComment: "ダウンロードコンテンツで足された夜の王か。");

            migrationBuilder.AlterColumn<string>(
                name: "image_path",
                schema: "game",
                table: "bosses",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldComment: "画像のパス。null は画像なし。");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                schema: "game",
                table: "bosses",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()",
                oldComment: "行を作った日時(UTC)。データベースの既定値で入る。");

            migrationBuilder.AlterColumn<string>(
                name: "id",
                schema: "game",
                table: "bosses",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldComment: "識別子。英小文字の固定の値。例: gladius。");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                schema: "game",
                table: "boss_weaknesses",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()",
                oldComment: "行を作った日時(UTC)。データベースの既定値で入る。");

            migrationBuilder.AlterColumn<string>(
                name: "boss_id",
                schema: "game",
                table: "boss_weaknesses",
                type: "character varying(50)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldComment: "夜の王の識別子。");

            migrationBuilder.AlterColumn<string>(
                name: "attribute",
                schema: "game",
                table: "boss_weaknesses",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30,
                oldComment: "弱点の属性。例: holy、fire。");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                schema: "game",
                table: "boss_weaknesses",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldComment: "識別子。")
                .Annotation("Npgsql:IdentitySequenceOptions", "'1001', '1', '', '', 'False', '1'")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:IdentitySequenceOptions", "'1001', '1', '', '', 'False', '1'")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<decimal>(
                name: "y",
                schema: "game",
                table: "base_points",
                type: "numeric(7,2)",
                precision: 7,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(7,2)",
                oldPrecision: 7,
                oldScale: 2,
                oldComment: "y 座標。1024 四方の座標空間の数。");

            migrationBuilder.AlterColumn<decimal>(
                name: "x",
                schema: "game",
                table: "base_points",
                type: "numeric(7,2)",
                precision: 7,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(7,2)",
                oldPrecision: 7,
                oldScale: 2,
                oldComment: "x 座標。1024 四方の座標空間の数。");

            migrationBuilder.AlterColumn<string>(
                name: "name_ja",
                schema: "game",
                table: "base_points",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "日本語の名前。null は名前なし。");

            migrationBuilder.AlterColumn<string>(
                name: "map_id",
                schema: "game",
                table: "base_points",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "normal",
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldDefaultValue: "normal",
                oldComment: "どのマップの拠点か。normal は通常のマップ、ほかは地変の識別子。");

            migrationBuilder.AlterColumn<DateTime>(
                name: "hidden_at",
                schema: "game",
                table: "base_points",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true,
                oldComment: "非表示にした日時。null は表示中。");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                schema: "game",
                table: "base_points",
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
                table: "base_points",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldComment: "識別子。初期データは 1000 以下、管理の画面で足した行は 1001 から。")
                .Annotation("Npgsql:IdentitySequenceOptions", "'1001', '1', '', '', 'False', '1'")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:IdentitySequenceOptions", "'1001', '1', '', '', 'False', '1'")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);
        }
    }
}
