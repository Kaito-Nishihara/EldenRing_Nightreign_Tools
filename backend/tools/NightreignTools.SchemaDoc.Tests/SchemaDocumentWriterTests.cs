namespace NightreignTools.SchemaDoc.Tests;

/// <summary>
/// テーブルの定義書の中身(データベースの共通仕様 5.8)。
/// </summary>
public class SchemaDocumentWriterTests
{
    private static string Write(string service) =>
        SchemaDocumentWriter.Write(ServiceSchema.All.Single(schema => schema.Service == service));

    [Fact]
    public void AC_DB_12_masterの定義書のshrink_zonesにname_jaが型と文字数とNULLと説明付きで載る()
    {
        var document = Write("master");

        var section = Section(document, "## game.shrink_zones");
        Assert.Contains("| `name_ja` | `character varying(100)` | 許さない |  | 日本語の名前。 |", section);
    }

    [Fact]
    public void 主キーの列が先に並び識別子は自動の採番と書く()
    {
        var section = Section(Write("master"), "## game.shrink_zones");

        var rows = section.Split('\n').Where(line => line.StartsWith("| `")).ToList();
        Assert.StartsWith("| `id` | `integer` | 許さない | 自動の採番(identity) |", rows[0]);
    }

    [Fact]
    public void 外部キーと親を消したときの動きと検査の制約が載る()
    {
        var section = Section(Write("record"), "## game.shrink_entries");

        Assert.Contains("| `play_record_id` | `game.play_records`(`id`) | 子も消す(CASCADE) |", section);
        Assert.Contains("`ck_shrink_entries_phase`", section);
    }

    [Fact]
    public void 同じモデルからは同じ定義書を作る()
    {
        Assert.Equal(Write("record"), Write("record"));
    }

    /// <summary>見出しから次の見出しの前までを返す。</summary>
    private static string Section(string document, string heading)
    {
        var start = document.IndexOf(heading, StringComparison.Ordinal);
        Assert.True(start >= 0, $"{heading} がない");
        var end = document.IndexOf("\n## ", start + heading.Length, StringComparison.Ordinal);
        return end < 0 ? document[start..] : document[start..end];
    }
}
