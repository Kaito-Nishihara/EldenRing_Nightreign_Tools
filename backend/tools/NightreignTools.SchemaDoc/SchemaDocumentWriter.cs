using System.Globalization;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace NightreignTools.SchemaDoc;

/// <summary>
/// 1 つのサービスのモデルから、テーブルの定義書の Markdown を作る(データベースの共通仕様 5.8)。
/// 同じモデルからは、いつも同じ文字列を作る。CI で作り直して比べるためである。
/// </summary>
internal static class SchemaDocumentWriter
{
    public static string Write(ServiceSchema schema)
    {
        var model = schema.LoadModel();
        var tables = model.GetRelationalModel().Tables
            .Where(table => !table.IsExcludedFromMigrations)
            .OrderBy(table => table.Name, StringComparer.Ordinal)
            .ToList();

        var builder = new StringBuilder();
        builder.AppendLine($"# テーブルの定義書: {schema.Service}-service");
        builder.AppendLine();
        builder.AppendLine("このファイルは `backend/tools/NightreignTools.SchemaDoc` がエンティティから作った生成物である。");
        builder.AppendLine("手で直さない。説明を変えるときは、Domain のエンティティの `<summary>` を直して作り直す([データベースの共通仕様](../design/common-database.md) 5.8)。");
        builder.AppendLine();
        builder.AppendLine($"- **データベース**: `{schema.Database}`");
        builder.AppendLine($"- **テーブル**: {tables.Count}");

        foreach (var table in tables)
        {
            WriteTable(builder, table);
        }

        return builder.ToString().ReplaceLineEndings("\n");
    }

    private static void WriteTable(StringBuilder builder, ITable table)
    {
        builder.AppendLine();
        builder.AppendLine($"## {table.Schema}.{table.Name}");
        builder.AppendLine();
        builder.AppendLine(Cell(table.Comment));
        builder.AppendLine();

        builder.AppendLine("| 列 | 型 | NULL | 既定値 | 説明 |");
        builder.AppendLine("|---|---|---|---|---|");
        foreach (var column in OrderColumns(table))
        {
            builder.AppendLine(
                $"| `{column.Name}` | `{column.StoreType}` | {(column.IsNullable ? "許す" : "許さない")} | {DefaultValue(column)} | {Cell(column.Comment)} |");
        }

        builder.AppendLine();
        builder.AppendLine("主キー: " + (table.PrimaryKey is { } key ? Columns(key.Columns) : "なし"));

        WriteSection(builder, "外部キー", "| 列 | 参照先 | 親を消したとき |",
            table.ForeignKeyConstraints
                .OrderBy(fk => fk.Name, StringComparer.Ordinal)
                .Select(fk => $"| {Columns(fk.Columns)} | `{fk.PrincipalTable.Schema}.{fk.PrincipalTable.Name}`({Columns(fk.PrincipalColumns)}) | {OnDelete(fk.OnDeleteAction)} |"));

        WriteSection(builder, "索引", "| 名前 | 列 | 一意 |",
            table.Indexes
                .OrderBy(index => index.Name, StringComparer.Ordinal)
                .Select(index => $"| `{index.Name}` | {Columns(index.Columns)} | {(index.IsUnique ? "一意" : "")} |"));

        WriteSection(builder, "検査の制約", "| 名前 | 式 |",
            table.CheckConstraints
                .OrderBy(check => check.Name, StringComparer.Ordinal)
                .Select(check => $"| `{check.Name}` | `{check.Sql}` |"));
    }

    /// <summary>
    /// 表の見出しと行を書く。
    /// 行がなければ「なし」と書く。
    /// </summary>
    /// <param name="builder">書き足す先。</param>
    /// <param name="title">表の題。</param>
    /// <param name="header">表の見出しの行。区切りの行はここで足す。</param>
    /// <param name="rows">表の行。空なら「なし」とだけ書く。</param>
    private static void WriteSection(StringBuilder builder, string title, string header, IEnumerable<string> rows)
    {
        var lines = rows.ToList();
        builder.AppendLine();
        if (lines.Count == 0)
        {
            builder.AppendLine($"{title}: なし");
            return;
        }

        builder.AppendLine($"{title}:");
        builder.AppendLine();
        builder.AppendLine(header);
        builder.AppendLine(string.Concat(Enumerable.Repeat("|---", header.Count(c => c == '|') - 1)) + "|");
        foreach (var line in lines)
        {
            builder.AppendLine(line);
        }
    }

    /// <summary>
    /// 主キーの列を先に、残りはエンティティで宣言した順(基底のクラスが先)に並べる。
    /// モデルの列は名前の順に並んでいて、エンティティを読むときの順と違うためである。
    /// </summary>
    /// <param name="table">列を並べるテーブル。</param>
    /// <returns>主キー、基底のクラスの列、エンティティの列の順に並べた列。</returns>
    private static IEnumerable<IColumn> OrderColumns(ITable table)
    {
        var keyColumns = table.PrimaryKey?.Columns.ToHashSet() ?? [];
        return table.Columns
            .OrderBy(column => keyColumns.Contains(column) ? 0 : 1)
            .ThenBy(column => InheritanceDepth(column))
            .ThenBy(column => column.PropertyMappings.First().Property.PropertyInfo?.MetadataToken ?? int.MaxValue)
            .ThenBy(column => column.Name, StringComparer.Ordinal);
    }

    private static int InheritanceDepth(IColumn column)
    {
        var depth = 0;
        for (var type = column.PropertyMappings.First().Property.PropertyInfo?.DeclaringType?.BaseType; type is not null; type = type.BaseType)
        {
            depth++;
        }

        return depth;
    }

    private static string DefaultValue(IColumn column)
    {
        var property = column.PropertyMappings.First().Property;
        if (property.GetValueGenerationStrategy() is NpgsqlValueGenerationStrategy.IdentityByDefaultColumn or NpgsqlValueGenerationStrategy.IdentityAlwaysColumn)
        {
            return "自動の採番(identity)";
        }

        if (column.DefaultValueSql is { } sql)
        {
            return $"`{sql}`";
        }

        return column.DefaultValue is { } value ? $"`{Convert.ToString(value, CultureInfo.InvariantCulture)}`" : "";
    }

    private static string OnDelete(ReferentialAction action) => action switch
    {
        ReferentialAction.Cascade => "子も消す(CASCADE)",
        ReferentialAction.Restrict => "消せない(RESTRICT)",
        ReferentialAction.SetNull => "null にする(SET NULL)",
        ReferentialAction.SetDefault => "既定値にする(SET DEFAULT)",
        _ => "何もしない(NO ACTION)",
    };

    private static string Columns(IEnumerable<IColumn> columns) =>
        string.Join("、", columns.Select(column => $"`{column.Name}`"));

    /// <summary>
    /// 表のセルに入れる文字。
    /// 縦線は表の区切りと読まれるので逃がす。
    /// </summary>
    /// <param name="text">セルに入れる文字。null は空にする。</param>
    /// <returns>縦線を逃がした文字。</returns>
    private static string Cell(string? text) => (text ?? "").Replace("|", "\\|");
}
