using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace NightreignTools.ServiceDefaults.Persistence;

/// <summary>
/// Domain のエンティティに書いた説明を、テーブルと列のコメントにする(データベースの共通仕様 5.8)。
/// 各サービスの DbContext の OnModelCreating の終わりで呼ぶ。
/// </summary>
public static class DocumentationCommentExtensions
{
    public static ModelBuilder ApplyDocumentationComments(this ModelBuilder modelBuilder, Assembly domainAssembly)
    {
        var documentation = XmlDocumentation.Load(domainAssembly);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (entityType.ClrType.Assembly != domainAssembly)
            {
                continue;
            }

            entityType.SetComment(documentation.GetSummary(entityType.ClrType));

            foreach (var property in entityType.GetProperties())
            {
                if (property.PropertyInfo is { } propertyInfo)
                {
                    property.SetComment(documentation.GetSummary(propertyInfo));
                }
            }
        }

        return modelBuilder;
    }

    /// <summary>
    /// 説明のないテーブルと列を、"テーブル" か "テーブル.列" の形で返す。
    /// どのテーブルとどの列にも説明を書く決まりを、テストで確かめるために使う(AC-DB-10)。
    /// </summary>
    /// <param name="model">説明を確かめる EF Core のモデル。設計時のモデルを渡す。</param>
    /// <returns>説明のないテーブルと列。すべてに説明があれば空。</returns>
    public static IReadOnlyList<string> FindMissingComments(IModel model)
    {
        var missing = new List<string>();
        foreach (var entityType in model.GetEntityTypes())
        {
            var table = entityType.GetTableName();
            if (table is null)
            {
                continue;
            }

            if (string.IsNullOrWhiteSpace(entityType.GetComment()))
            {
                missing.Add(table);
            }

            var storeObject = StoreObjectIdentifier.Table(table, entityType.GetSchema());
            missing.AddRange(entityType.GetProperties()
                .Where(property => string.IsNullOrWhiteSpace(property.GetComment()))
                .Select(property => $"{table}.{property.GetColumnName(storeObject)}"));
        }

        return missing;
    }
}
