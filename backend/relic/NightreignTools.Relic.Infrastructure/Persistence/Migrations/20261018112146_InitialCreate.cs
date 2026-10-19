using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NightreignTools.Relic.Infrastructure.Persistence.Migrations
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
                name: "relic_effect_categories",
                schema: "game",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, comment: "識別子。英小文字の固定の値。例: attack。"),
                    name_ja = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false, comment: "日本語の名前。"),
                    sort_order = table.Column<int>(type: "integer", nullable: false, comment: "画面での並び順。小さいほうが先(ビルドシミュレーター 3.2)。"),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()", comment: "行を作った日時(UTC)。データベースの既定値で入る。")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_relic_effect_categories", x => x.id);
                },
                comment: "遺物の効果の分類。例: 能力値、攻撃力。マイグレーションの初期データだけで管理する。");

            migrationBuilder.CreateTable(
                name: "relic_effects",
                schema: "game",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false, comment: "識別子。")
                        .Annotation("Npgsql:IdentitySequenceOptions", "'1001', '1', '', '', 'False', '1'")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    category_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, comment: "分類の識別子。"),
                    name_ja = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false, comment: "日本語の名前。"),
                    is_deep = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "深き夜の遺物に付く効果(深層効果)か。"),
                    effect_sign = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "positive", comment: "効果がメリットかデメリットか。positive か negative。"),
                    calc_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, comment: "効果を重ねるときの合わせ方。multiply、percent_add、flat_add、special のどれか。"),
                    attribute = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true, comment: "効果がかかる対象。例: physical、strength。null は効果ごとに別に合わせる。"),
                    stackable = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "yes", comment: "同じ効果を重ねて付けられるか。yes、no、conditional(条件付き)のどれか。"),
                    rank_min = table.Column<int>(type: "integer", nullable: false, comment: "ランクの下限。ランクのない効果は 0。"),
                    rank_max = table.Column<int>(type: "integer", nullable: false, comment: "ランクの上限。ランクのない効果は 0。"),
                    condition_ja = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true, comment: "発動の条件。例: HP40%未満で発動。null は条件なし。"),
                    notes_ja = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true, comment: "備考。null はなし。"),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()", comment: "行を作った日時(UTC)。データベースの既定値で入る。")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_relic_effects", x => x.id);
                    table.CheckConstraint("ck_relic_effects_calc_type", "calc_type IN ('multiply', 'percent_add', 'flat_add', 'special')");
                    table.CheckConstraint("ck_relic_effects_effect_sign", "effect_sign IN ('positive', 'negative')");
                    table.CheckConstraint("ck_relic_effects_rank", "rank_min >= 0 AND rank_max >= rank_min");
                    table.CheckConstraint("ck_relic_effects_stackable", "stackable IN ('yes', 'no', 'conditional')");
                    table.ForeignKey(
                        name: "fk_relic_effects_relic_effect_categories_category_id",
                        column: x => x.category_id,
                        principalSchema: "game",
                        principalTable: "relic_effect_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "遺物に付く 1 つの効果。通常効果と深層効果を同じテーブルで持ち、深層効果かで分ける。マイグレーションの初期データだけで管理する。");

            migrationBuilder.CreateTable(
                name: "relic_effect_ranks",
                schema: "game",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false, comment: "識別子。")
                        .Annotation("Npgsql:IdentitySequenceOptions", "'1001', '1', '', '', 'False', '1'")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    effect_id = table.Column<int>(type: "integer", nullable: false, comment: "効果の識別子。"),
                    rank = table.Column<int>(type: "integer", nullable: false, comment: "ランク。ランクのない効果は 0。"),
                    value = table.Column<decimal>(type: "numeric(10,4)", precision: 10, scale: 4, nullable: true, comment: "このランクの値。単位は Unit による。null は数で表せない効果(special)。"),
                    unit = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, comment: "値の単位。multiplier(倍率)、percent(% の点数)、flat(固定値)、per_sec(1 秒ごと)、none(数なし)のどれか。"),
                    has_demerit = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "このランクからデメリットが付くか。深層効果で使う。"),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()", comment: "行を作った日時(UTC)。データベースの既定値で入る。")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_relic_effect_ranks", x => x.id);
                    table.CheckConstraint("ck_relic_effect_ranks_rank", "rank >= 0");
                    table.CheckConstraint("ck_relic_effect_ranks_unit", "unit IN ('multiplier', 'percent', 'flat', 'per_sec', 'none')");
                    table.ForeignKey(
                        name: "fk_relic_effect_ranks_relic_effects_effect_id",
                        column: x => x.effect_id,
                        principalSchema: "game",
                        principalTable: "relic_effects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "遺物の効果のランクごとの値。効果とランクの組は 1 行だけ。");

            migrationBuilder.InsertData(
                schema: "game",
                table: "relic_effect_categories",
                columns: new[] { "id", "created_at", "name_ja", "sort_order" },
                values: new object[,]
                {
                    { "arts_gauge", new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "アーツゲージ", 5 },
                    { "attack", new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "攻撃力", 1 },
                    { "cut_rate", new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "カット率", 2 },
                    { "other", new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "その他", 7 },
                    { "recovery", new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "回復", 3 },
                    { "starting_item", new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "出撃装備", 6 },
                    { "stat", new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "能力値", 0 },
                    { "status_resist", new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "状態異常耐性", 4 }
                });

            migrationBuilder.InsertData(
                schema: "game",
                table: "relic_effects",
                columns: new[] { "id", "attribute", "calc_type", "category_id", "condition_ja", "created_at", "effect_sign", "name_ja", "notes_ja", "rank_max", "rank_min", "stackable" },
                values: new object[,]
                {
                    { 1, null, "flat_add", "stat", null, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "positive", "最大HP上昇", null, 0, 0, "no" },
                    { 2, null, "flat_add", "stat", null, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "positive", "最大FP上昇", null, 0, 0, "yes" },
                    { 3, null, "flat_add", "stat", null, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "positive", "最大スタミナ上昇", null, 0, 0, "yes" },
                    { 4, "vigor", "flat_add", "stat", null, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "positive", "生命力+1〜3", null, 3, 1, "yes" },
                    { 5, "mind", "flat_add", "stat", null, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "positive", "精神力+1〜3", null, 3, 1, "yes" },
                    { 6, "endurance", "flat_add", "stat", null, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "positive", "持久力+1〜3", null, 3, 1, "yes" },
                    { 7, "strength", "flat_add", "stat", null, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "positive", "筋力+1〜3", null, 3, 1, "yes" },
                    { 8, "dexterity", "flat_add", "stat", null, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "positive", "技量+1〜3", null, 3, 1, "yes" },
                    { 9, "intelligence", "flat_add", "stat", null, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "positive", "知力+1〜3", null, 3, 1, "yes" },
                    { 10, "faith", "flat_add", "stat", null, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "positive", "信仰+1〜3", null, 3, 1, "yes" },
                    { 11, "arcane", "flat_add", "stat", null, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "positive", "神秘+1〜3", null, 3, 1, "yes" },
                    { 12, "toughness", "percent_add", "stat", null, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "positive", "強靭度+1〜3", null, 3, 1, "yes" },
                    { 13, "physical", "percent_add", "attack", null, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "positive", "物理攻撃力上昇+0〜2", null, 2, 0, "yes" },
                    { 14, "magic", "percent_add", "attack", null, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "positive", "魔力攻撃力上昇+0〜2", null, 2, 0, "yes" },
                    { 15, "fire", "percent_add", "attack", null, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "positive", "炎攻撃力上昇+0〜2", null, 2, 0, "yes" },
                    { 16, "lightning", "percent_add", "attack", null, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "positive", "雷攻撃力上昇+0〜2", null, 2, 0, "yes" },
                    { 17, "holy", "percent_add", "attack", null, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "positive", "聖攻撃力上昇+0〜2", null, 2, 0, "yes" },
                    { 18, "melee", "multiply", "attack", null, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "positive", "近接攻撃力上昇", null, 0, 0, "yes" },
                    { 19, "skill", "multiply", "attack", null, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "positive", "戦技攻撃力上昇", null, 0, 0, "yes" },
                    { 20, null, "multiply", "attack", null, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "positive", "通常攻撃の1段目強化", null, 0, 0, "yes" },
                    { 21, null, "multiply", "attack", null, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "positive", "致命の一撃強化", null, 0, 0, "yes" },
                    { 22, null, "multiply", "attack", null, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "positive", "致命の一撃強化+1", null, 0, 0, "no" },
                    { 23, null, "multiply", "attack", null, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "positive", "ガードカウンター強化", null, 0, 0, "yes" },
                    { 24, "breath", "multiply", "attack", null, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "positive", "咆哮とブレス強化", null, 0, 0, "yes" },
                    { 25, "throw_pot", "multiply", "attack", null, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "positive", "投擲壺の攻撃力上昇", null, 0, 0, "yes" },
                    { 26, "throw_knife", "multiply", "attack", null, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "positive", "投擲ナイフの攻撃力上昇", null, 0, 0, "yes" },
                    { 27, null, "special", "attack", null, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "positive", "攻撃連続時、攻撃力上昇", "段階的に上昇: 1段目×1.05 → 累計×1.12 → 累計×1.22", 0, 0, "conditional" },
                    { 28, "physical", "percent_add", "cut_rate", null, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "positive", "物理カット率上昇", null, 0, 0, "no" },
                    { 29, "magic", "percent_add", "cut_rate", null, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "positive", "魔力カット率上昇", null, 0, 0, "yes" },
                    { 30, "fire", "percent_add", "cut_rate", null, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "positive", "炎カット率上昇", null, 0, 0, "yes" },
                    { 31, "lightning", "percent_add", "cut_rate", null, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "positive", "雷カット率上昇", null, 0, 0, "yes" },
                    { 32, "holy", "percent_add", "cut_rate", null, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "positive", "聖カット率上昇", null, 0, 0, "yes" },
                    { 33, null, "percent_add", "cut_rate", "HP40%未満で発動", new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "positive", "HP低下時、カット率上昇", null, 0, 0, "conditional" },
                    { 34, null, "special", "cut_rate", null, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "positive", "ダメージで吹き飛ばされた時、強靭度とカット率上昇", "カット率+20%・強靭度+20%、20秒持続", 0, 0, "no" },
                    { 35, "poison", "flat_add", "status_resist", null, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "positive", "毒耐性上昇", null, 0, 0, "yes" },
                    { 36, "rot", "flat_add", "status_resist", null, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "positive", "腐敗耐性上昇", null, 0, 0, "yes" },
                    { 37, "bleed", "flat_add", "status_resist", null, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "positive", "出血耐性上昇", null, 0, 0, "yes" },
                    { 38, "frostbite", "flat_add", "status_resist", null, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "positive", "冷気耐性上昇", null, 0, 0, "yes" },
                    { 39, "sleep", "flat_add", "status_resist", null, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "positive", "睡眠耐性上昇", null, 0, 0, "yes" },
                    { 40, "madness", "flat_add", "status_resist", null, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "positive", "発狂耐性上昇", null, 0, 0, "yes" },
                    { 41, "death", "flat_add", "status_resist", null, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "positive", "抗死耐性上昇", null, 0, 0, "yes" },
                    { 42, null, "flat_add", "recovery", null, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "positive", "HP持続回復", null, 0, 0, "yes" },
                    { 43, null, "special", "recovery", "HP50%以下で発動", new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "positive", "HP低下時、周囲の味方を含めHPをゆっくり回復", "最大HP0.5%+1/秒、50秒持続", 0, 0, "no" },
                    { 44, null, "flat_add", "recovery", null, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "positive", "ガード成功時、HP回復", "クールタイムあり", 0, 0, "no" },
                    { 45, null, "percent_add", "recovery", null, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "positive", "刺突カウンター発生時、HP回復", null, 0, 0, "no" },
                    { 46, null, "special", "recovery", "発狂状態中のみ", new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "positive", "発狂状態になると、FP持続回復", "+2/秒、25秒間（合計+50）", 0, 0, "conditional" },
                    { 47, null, "special", "recovery", null, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "positive", "攻撃命中時、スタミナ回復", "1ヒットにつき約2〜3回復", 0, 0, "no" },
                    { 48, null, "special", "recovery", null, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "positive", "致命の一撃で、スタミナ回復速度上昇", "+15%、15秒間", 0, 0, "no" },
                    { 49, null, "special", "recovery", null, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "positive", "リゲイン有効化", "ダメージ後の攻撃で一部HP回復", 0, 0, "no" },
                    { 50, null, "percent_add", "arts_gauge", null, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "positive", "スキルクールタイム軽減+1〜3", null, 3, 1, "yes" },
                    { 51, null, "percent_add", "arts_gauge", null, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "positive", "アーツゲージ自然蓄積+1〜3", null, 3, 1, "yes" },
                    { 52, null, "special", "arts_gauge", null, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "positive", "敵撃破時、アーツゲージ増加", "約5%増加", 0, 0, "no" },
                    { 53, null, "special", "arts_gauge", null, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "positive", "致命の一撃でアーツゲージ増加", "約5%増加", 0, 0, "no" }
                });

            migrationBuilder.InsertData(
                schema: "game",
                table: "relic_effects",
                columns: new[] { "id", "attribute", "calc_type", "category_id", "condition_ja", "created_at", "effect_sign", "is_deep", "name_ja", "notes_ja", "rank_max", "rank_min", "stackable" },
                values: new object[,]
                {
                    { 54, null, "percent_add", "stat", null, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "positive", true, "最大HP上昇+10%", null, 0, 0, "yes" },
                    { 55, null, "percent_add", "stat", null, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "positive", true, "最大FP上昇+15%", null, 0, 0, "yes" },
                    { 56, null, "percent_add", "stat", null, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "positive", true, "最大スタミナ上昇+12%", null, 0, 0, "yes" },
                    { 57, "physical", "percent_add", "attack", null, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "positive", true, "物理攻撃力上昇+0〜4（深層）", null, 4, 0, "yes" },
                    { 58, "magic", "percent_add", "attack", null, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "positive", true, "魔力攻撃力上昇+0〜2（深層）", null, 2, 0, "yes" },
                    { 59, "fire", "percent_add", "attack", null, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "positive", true, "炎攻撃力上昇+0〜2（深層）", null, 2, 0, "yes" },
                    { 60, "lightning", "percent_add", "attack", null, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "positive", true, "雷攻撃力上昇+0〜2（深層）", null, 2, 0, "yes" },
                    { 61, "holy", "percent_add", "attack", null, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "positive", true, "聖攻撃力上昇+0〜2（深層）", null, 2, 0, "yes" },
                    { 62, null, "multiply", "attack", null, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "positive", true, "ガードカウンター強化+0〜2（深層）", null, 2, 0, "yes" },
                    { 63, null, "flat_add", "recovery", null, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "negative", true, "HP持続減少", null, 0, 0, "yes" },
                    { 64, "all_status", "flat_add", "status_resist", null, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "negative", true, "すべての状態異常耐性低下", null, 0, 0, "yes" },
                    { 65, "vigor_arcane", "flat_add", "stat", null, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "negative", true, "生命力と神秘が低下", null, 0, 0, "yes" },
                    { 66, "strength_intelligence", "flat_add", "stat", null, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "negative", true, "筋力と知力が低下", null, 0, 0, "yes" },
                    { 67, "dexterity_faith", "flat_add", "stat", null, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "negative", true, "技量と信仰が低下", null, 0, 0, "yes" },
                    { 68, "intelligence_dexterity", "flat_add", "stat", null, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "negative", true, "知力と技量が低下", null, 0, 0, "yes" },
                    { 69, "faith_strength", "flat_add", "stat", null, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "negative", true, "信仰と筋力が低下", null, 0, 0, "yes" },
                    { 70, null, "multiply", "other", null, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "negative", true, "取得ルーン減少", null, 0, 0, "yes" },
                    { 71, "poison", "special", "status_resist", null, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "negative", true, "被ダメージ時、毒を蓄積", null, 0, 0, "yes" },
                    { 72, "rot", "special", "status_resist", null, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "negative", true, "被ダメージ時、腐敗を蓄積", null, 0, 0, "yes" },
                    { 73, "bleed", "special", "status_resist", null, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "negative", true, "被ダメージ時、出血を蓄積", null, 0, 0, "yes" },
                    { 74, "frostbite", "special", "status_resist", null, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "negative", true, "被ダメージ時、冷気を蓄積", null, 0, 0, "yes" },
                    { 75, null, "percent_add", "cut_rate", null, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), "negative", true, "回避連続時、カット率低下", "-35%、15秒持続", 0, 0, "conditional" }
                });

            migrationBuilder.InsertData(
                schema: "game",
                table: "relic_effect_ranks",
                columns: new[] { "id", "created_at", "effect_id", "rank", "unit", "value" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 1, 0, "flat", 100m },
                    { 2, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 2, 0, "flat", 25m },
                    { 3, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 3, 0, "flat", 10m },
                    { 4, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 4, 1, "flat", 1m },
                    { 5, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 4, 2, "flat", 2m },
                    { 6, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 4, 3, "flat", 3m },
                    { 7, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 5, 1, "flat", 1m },
                    { 8, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 5, 2, "flat", 2m },
                    { 9, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 5, 3, "flat", 3m },
                    { 10, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 6, 1, "flat", 1m },
                    { 11, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 6, 2, "flat", 2m },
                    { 12, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 6, 3, "flat", 3m },
                    { 13, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 7, 1, "flat", 1m },
                    { 14, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 7, 2, "flat", 2m },
                    { 15, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 7, 3, "flat", 3m },
                    { 16, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 8, 1, "flat", 1m },
                    { 17, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 8, 2, "flat", 2m },
                    { 18, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 8, 3, "flat", 3m },
                    { 19, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 9, 1, "flat", 1m },
                    { 20, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 9, 2, "flat", 2m },
                    { 21, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 9, 3, "flat", 3m },
                    { 22, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 10, 1, "flat", 1m },
                    { 23, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 10, 2, "flat", 2m },
                    { 24, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 10, 3, "flat", 3m },
                    { 25, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 11, 1, "flat", 1m },
                    { 26, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 11, 2, "flat", 2m },
                    { 27, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 11, 3, "flat", 3m },
                    { 28, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 12, 1, "percent", 5m },
                    { 29, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 12, 2, "percent", 10m },
                    { 30, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 12, 3, "percent", 15m },
                    { 31, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 13, 0, "percent", 4m },
                    { 32, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 13, 1, "percent", 5m },
                    { 33, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 13, 2, "percent", 6m },
                    { 34, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 14, 0, "percent", 4m },
                    { 35, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 14, 1, "percent", 5m },
                    { 36, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 14, 2, "percent", 6m },
                    { 37, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 15, 0, "percent", 4m },
                    { 38, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 15, 1, "percent", 5m },
                    { 39, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 15, 2, "percent", 6m },
                    { 40, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 16, 0, "percent", 4m },
                    { 41, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 16, 1, "percent", 5m },
                    { 42, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 16, 2, "percent", 6m },
                    { 43, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 17, 0, "percent", 4m },
                    { 44, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 17, 1, "percent", 5m },
                    { 45, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 17, 2, "percent", 6m },
                    { 46, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 18, 0, "multiplier", 1.05m },
                    { 47, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 19, 0, "multiplier", 1.15m },
                    { 48, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 20, 0, "multiplier", 1.15m },
                    { 49, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 21, 0, "multiplier", 1.17m },
                    { 50, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 22, 0, "multiplier", 1.24m },
                    { 51, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 23, 0, "multiplier", 1.17m },
                    { 52, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 24, 0, "multiplier", 1.15m },
                    { 53, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 25, 0, "multiplier", 1.15m },
                    { 54, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 26, 0, "multiplier", 1.14m },
                    { 55, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 27, 0, "none", null },
                    { 56, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 28, 0, "percent", 8m },
                    { 57, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 29, 0, "percent", 10m },
                    { 58, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 30, 0, "percent", 10m },
                    { 59, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 31, 0, "percent", 10m },
                    { 60, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 32, 0, "percent", 10m },
                    { 61, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 33, 0, "percent", 15m },
                    { 62, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 34, 0, "none", null },
                    { 63, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 35, 0, "flat", 75m },
                    { 64, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 36, 0, "flat", 75m },
                    { 65, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 37, 0, "flat", 75m },
                    { 66, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 38, 0, "flat", 75m },
                    { 67, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 39, 0, "flat", 75m },
                    { 68, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 40, 0, "flat", 75m },
                    { 69, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 41, 0, "flat", 75m },
                    { 70, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 42, 0, "per_sec", 2m },
                    { 71, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 43, 0, "none", null },
                    { 72, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 44, 0, "flat", 15m },
                    { 73, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 45, 0, "percent", 2.5m },
                    { 74, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 46, 0, "per_sec", 2m },
                    { 75, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 47, 0, "none", null },
                    { 76, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 48, 0, "none", null },
                    { 77, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 49, 0, "none", null },
                    { 78, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 50, 1, "percent", 5m },
                    { 79, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 50, 2, "percent", 7.5m },
                    { 80, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 50, 3, "percent", 10m },
                    { 81, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 51, 1, "percent", 5m },
                    { 82, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 51, 2, "percent", 7.5m },
                    { 83, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 51, 3, "percent", 10m },
                    { 84, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 52, 0, "none", null },
                    { 85, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 53, 0, "none", null }
                });

            migrationBuilder.InsertData(
                schema: "game",
                table: "relic_effect_ranks",
                columns: new[] { "id", "created_at", "effect_id", "has_demerit", "rank", "unit", "value" },
                values: new object[,]
                {
                    { 86, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 54, true, 0, "percent", 10m },
                    { 87, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 55, true, 0, "percent", 15m },
                    { 88, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 56, true, 0, "percent", 12m }
                });

            migrationBuilder.InsertData(
                schema: "game",
                table: "relic_effect_ranks",
                columns: new[] { "id", "created_at", "effect_id", "rank", "unit", "value" },
                values: new object[,]
                {
                    { 89, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 57, 0, "percent", 4m },
                    { 90, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 57, 1, "percent", 5m },
                    { 91, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 57, 2, "percent", 6m }
                });

            migrationBuilder.InsertData(
                schema: "game",
                table: "relic_effect_ranks",
                columns: new[] { "id", "created_at", "effect_id", "has_demerit", "rank", "unit", "value" },
                values: new object[,]
                {
                    { 92, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 57, true, 3, "percent", 10.5m },
                    { 93, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 57, true, 4, "percent", 12m }
                });

            migrationBuilder.InsertData(
                schema: "game",
                table: "relic_effect_ranks",
                columns: new[] { "id", "created_at", "effect_id", "rank", "unit", "value" },
                values: new object[] { 94, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 58, 0, "percent", 5m });

            migrationBuilder.InsertData(
                schema: "game",
                table: "relic_effect_ranks",
                columns: new[] { "id", "created_at", "effect_id", "has_demerit", "rank", "unit", "value" },
                values: new object[,]
                {
                    { 95, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 58, true, 1, "percent", 8m },
                    { 96, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 58, true, 2, "percent", 10m }
                });

            migrationBuilder.InsertData(
                schema: "game",
                table: "relic_effect_ranks",
                columns: new[] { "id", "created_at", "effect_id", "rank", "unit", "value" },
                values: new object[] { 97, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 59, 0, "percent", 5m });

            migrationBuilder.InsertData(
                schema: "game",
                table: "relic_effect_ranks",
                columns: new[] { "id", "created_at", "effect_id", "has_demerit", "rank", "unit", "value" },
                values: new object[,]
                {
                    { 98, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 59, true, 1, "percent", 8m },
                    { 99, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 59, true, 2, "percent", 10m }
                });

            migrationBuilder.InsertData(
                schema: "game",
                table: "relic_effect_ranks",
                columns: new[] { "id", "created_at", "effect_id", "rank", "unit", "value" },
                values: new object[] { 100, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 60, 0, "percent", 5m });

            migrationBuilder.InsertData(
                schema: "game",
                table: "relic_effect_ranks",
                columns: new[] { "id", "created_at", "effect_id", "has_demerit", "rank", "unit", "value" },
                values: new object[,]
                {
                    { 101, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 60, true, 1, "percent", 8m },
                    { 102, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 60, true, 2, "percent", 10m }
                });

            migrationBuilder.InsertData(
                schema: "game",
                table: "relic_effect_ranks",
                columns: new[] { "id", "created_at", "effect_id", "rank", "unit", "value" },
                values: new object[] { 103, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 61, 0, "percent", 5m });

            migrationBuilder.InsertData(
                schema: "game",
                table: "relic_effect_ranks",
                columns: new[] { "id", "created_at", "effect_id", "has_demerit", "rank", "unit", "value" },
                values: new object[,]
                {
                    { 104, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 61, true, 1, "percent", 8m },
                    { 105, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 61, true, 2, "percent", 10m }
                });

            migrationBuilder.InsertData(
                schema: "game",
                table: "relic_effect_ranks",
                columns: new[] { "id", "created_at", "effect_id", "rank", "unit", "value" },
                values: new object[] { 106, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 62, 0, "multiplier", 1.17m });

            migrationBuilder.InsertData(
                schema: "game",
                table: "relic_effect_ranks",
                columns: new[] { "id", "created_at", "effect_id", "has_demerit", "rank", "unit", "value" },
                values: new object[,]
                {
                    { 107, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 62, true, 1, "multiplier", 1.25m },
                    { 108, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 62, true, 2, "multiplier", 1.29m }
                });

            migrationBuilder.InsertData(
                schema: "game",
                table: "relic_effect_ranks",
                columns: new[] { "id", "created_at", "effect_id", "rank", "unit", "value" },
                values: new object[,]
                {
                    { 109, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 63, 0, "per_sec", -2m },
                    { 110, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 64, 0, "flat", -80m },
                    { 111, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 65, 0, "flat", -3m },
                    { 112, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 66, 0, "flat", -3m },
                    { 113, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 67, 0, "flat", -3m },
                    { 114, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 68, 0, "flat", -3m },
                    { 115, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 69, 0, "flat", -3m },
                    { 116, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 70, 0, "multiplier", 0.9m },
                    { 117, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 71, 0, "flat", 65m },
                    { 118, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 72, 0, "flat", 55m },
                    { 119, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 73, 0, "flat", 55m },
                    { 120, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 74, 0, "flat", 55m },
                    { 121, new DateTime(2026, 10, 18, 0, 0, 0, 0, DateTimeKind.Utc), 75, 0, "percent", -35m }
                });

            migrationBuilder.CreateIndex(
                name: "ix_relic_effect_ranks_effect_id_rank",
                schema: "game",
                table: "relic_effect_ranks",
                columns: new[] { "effect_id", "rank" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_relic_effects_category_id",
                schema: "game",
                table: "relic_effects",
                column: "category_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "relic_effect_ranks",
                schema: "game");

            migrationBuilder.DropTable(
                name: "relic_effects",
                schema: "game");

            migrationBuilder.DropTable(
                name: "relic_effect_categories",
                schema: "game");
        }
    }
}
