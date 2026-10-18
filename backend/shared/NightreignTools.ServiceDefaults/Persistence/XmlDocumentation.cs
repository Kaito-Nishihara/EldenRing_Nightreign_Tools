using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace NightreignTools.ServiceDefaults.Persistence;

/// <summary>
/// アセンブリの XML ドキュメントのファイルから、型とプロパティの &lt;summary&gt; を読む(データベースの共通仕様 5.8)。
/// テーブルと列の説明を、Domain のエンティティに書いた説明から取るために使う。
/// </summary>
public sealed class XmlDocumentation
{
    private readonly Dictionary<string, string> _summaries;

    private XmlDocumentation(Dictionary<string, string> summaries) => _summaries = summaries;

    /// <summary>
    /// アセンブリの隣にある XML ドキュメントのファイルを読む。
    /// ないときは、説明が空のままモデルができてマイグレーションと食い違うので、ここで止める。
    /// </summary>
    public static XmlDocumentation Load(Assembly assembly)
    {
        var path = Path.ChangeExtension(assembly.Location, ".xml");
        if (!File.Exists(path))
        {
            throw new InvalidOperationException(
                $"{Path.GetFileName(path)} がありません。{assembly.GetName().Name} の csproj で GenerateDocumentationFile を true にしてください(データベースの共通仕様 5.8)");
        }

        return Parse(XDocument.Load(path));
    }

    /// <summary>XML ドキュメントの中身から読む。テストから使う。</summary>
    public static XmlDocumentation Parse(XDocument document)
    {
        var summaries = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var member in document.Descendants("member"))
        {
            var name = member.Attribute("name")?.Value;
            var summary = member.Element("summary");
            if (name is null || summary is null)
            {
                continue;
            }

            var text = ToPlainText(summary);
            if (text.Length > 0)
            {
                summaries[name] = text;
            }
        }

        return new XmlDocumentation(summaries);
    }

    /// <summary>型の説明。書いていなければ null。</summary>
    public string? GetSummary(Type type) =>
        _summaries.GetValueOrDefault($"T:{MemberName(type)}");

    /// <summary>プロパティの説明。基底のクラスで宣言したプロパティは、基底のクラスの説明を返す。書いていなければ null。</summary>
    public string? GetSummary(PropertyInfo property) =>
        property.DeclaringType is null ? null : _summaries.GetValueOrDefault($"P:{MemberName(property.DeclaringType)}.{property.Name}");

    /// <summary>入れ子の型は、XML ドキュメントでは + ではなく . でつなぐ。</summary>
    private static string MemberName(Type type) => (type.FullName ?? type.Name).Replace('+', '.');

    /// <summary>
    /// &lt;summary&gt; の中身を 1 行の文字列にする。
    /// 行の頭と終わりの空白を除いてつなぎ、&lt;see cref&gt; は型の短い名前に、&lt;c&gt; は中の文字にする。
    /// </summary>
    private static string ToPlainText(XElement summary)
    {
        var builder = new StringBuilder();
        foreach (var node in summary.Nodes())
        {
            builder.Append(node switch
            {
                XText text => text.Value,
                XElement { Name.LocalName: "see" or "seealso" } see => ShortName(see.Attribute("cref")?.Value ?? see.Attribute("langword")?.Value ?? ""),
                XElement element => element.Value,
                _ => "",
            });
        }

        var lines = builder.ToString()
            .Split('\n')
            .Select(line => line.Trim())
            .Where(line => line.Length > 0);

        // 日本語の文は空白を挟まずにつなぐ。英数字どうしが行をまたぐときだけ空白を挟む
        var result = new StringBuilder();
        foreach (var line in lines)
        {
            if (result.Length > 0 && char.IsAsciiLetterOrDigit(result[^1]) && char.IsAsciiLetterOrDigit(line[0]))
            {
                result.Append(' ');
            }

            result.Append(line);
        }

        return result.ToString();
    }

    /// <summary>"T:NightreignTools.Master.Domain.Entities.Boss" を "Boss" にする。</summary>
    private static string ShortName(string cref)
    {
        var name = cref.Contains(':') ? cref[(cref.IndexOf(':') + 1)..] : cref;
        return name[(name.LastIndexOf('.') + 1)..];
    }
}
