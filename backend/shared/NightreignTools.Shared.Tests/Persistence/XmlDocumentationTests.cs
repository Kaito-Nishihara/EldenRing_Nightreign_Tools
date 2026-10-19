using System.Reflection;
using System.Xml.Linq;
using NightreignTools.ServiceDefaults.Persistence;

namespace NightreignTools.Shared.Tests.Persistence;

/// <summary>
/// XML ドキュメントからテーブルと列の説明を読む処理(データベースの共通仕様 5.8)。
/// </summary>
public class XmlDocumentationTests
{
    private static XmlDocumentation Parse(string members) =>
        XmlDocumentation.Parse(XDocument.Parse($"<doc><members>{members}</members></doc>"));

    [Fact]
    public void 型の説明を読む()
    {
        var documentation = Parse(
            $"""<member name="T:{Name(typeof(Sample))}"><summary>見本のテーブル。</summary></member>""");

        Assert.Equal("見本のテーブル。", documentation.GetSummary(typeof(Sample)));
    }

    [Fact]
    public void 複数行の説明は行の頭と終わりの空白を除いて1行につなぐ()
    {
        var documentation = Parse(
            $"""
            <member name="P:{Name(typeof(Sample))}.Name">
              <summary>
              名前。
              100 文字まで。
              </summary>
            </member>
            """);

        Assert.Equal("名前。100 文字まで。", documentation.GetSummary(Property(nameof(Sample.Name))));
    }

    [Fact]
    public void 英数字どうしが行をまたぐときは空白を挟む()
    {
        var documentation = Parse(
            $"""
            <member name="P:{Name(typeof(Sample))}.Name">
              <summary>
              uses master
              service
              </summary>
            </member>
            """);

        Assert.Equal("uses master service", documentation.GetSummary(Property(nameof(Sample.Name))));
    }

    [Fact]
    public void seeとcは中の名前と文字にする()
    {
        var documentation = Parse(
            $"""<member name="P:{Name(typeof(Sample))}.Name"><summary><see cref="T:A.B.Boss"/> の <c>name_ja</c>。</summary></member>""");

        Assert.Equal("Boss の name_ja。", documentation.GetSummary(Property(nameof(Sample.Name))));
    }

    [Fact]
    public void 基底のクラスで宣言したプロパティは基底のクラスの説明を返す()
    {
        var documentation = Parse(
            $"""<member name="P:{Name(typeof(SampleBase))}.Id"><summary>識別子。</summary></member>""");

        Assert.Equal("識別子。", documentation.GetSummary(typeof(Sample).GetProperty(nameof(Sample.Id))!));
    }

    [Fact]
    public void 説明がないときはnullを返す()
    {
        var documentation = Parse("");

        Assert.Null(documentation.GetSummary(typeof(Sample)));
        Assert.Null(documentation.GetSummary(Property(nameof(Sample.Name))));
    }

    [Fact]
    public void XMLドキュメントのファイルがないアセンブリを読むと止まる()
    {
        // このテストのアセンブリは XML ドキュメントを出力しない
        var exception = Assert.Throws<InvalidOperationException>(() => XmlDocumentation.Load(Assembly.GetExecutingAssembly()));

        Assert.Contains("GenerateDocumentationFile", exception.Message);
    }

    /// <summary>
    /// XML ドキュメントの中の型の名前。
    /// 入れ子の型は + ではなく . でつなぐ。
    /// </summary>
    /// <param name="type">名前を作る型。</param>
    /// <returns>XML ドキュメントの member の name に使う型の名前。</returns>
    private static string Name(Type type) => type.FullName!.Replace('+', '.');

    private static PropertyInfo Property(string name) => typeof(Sample).GetProperty(name)!;

    private abstract class SampleBase
    {
        public int Id { get; set; }
    }

    private sealed class Sample : SampleBase
    {
        public string Name { get; set; } = "";
    }
}
