using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using NightreignTools.Master.Domain;
using NightreignTools.Master.Domain.Entities;
using NightreignTools.Master.Infrastructure.Persistence;
using NightreignTools.ServiceDefaults.Persistence;

namespace NightreignTools.Master.Tests.Integration;

/// <summary>
/// テーブルと列の説明(データベースの共通仕様 5.8)。説明はエンティティの &lt;summary&gt; から取る。
/// </summary>
public class SchemaCommentTests
{
    [Fact]
    public void AC_DB_10_masterのすべてのテーブルとすべての列に説明がある()
    {
        using var context = new DesignTimeDbContextFactory().CreateDbContext([]);
        var model = context.GetService<IDesignTimeModel>().Model;

        Assert.Empty(DocumentationCommentExtensions.FindMissingComments(model));
    }

    [Fact]
    public async Task AC_DB_11_マイグレーションを適用したデータベースの列のコメントがエンティティの説明と同じである()
    {
        using var factory = new MasterServiceFactory();
        factory.CreateClient();

        var comment = await TestDatabase.QueryAsync(
            "SELECT col_description('game.shrink_zones'::regclass, a.attnum) FROM pg_attribute a " +
            "WHERE a.attrelid = 'game.shrink_zones'::regclass AND a.attname = 'name_ja'");

        var expected = XmlDocumentation.Load(typeof(DomainAssembly).Assembly)
            .GetSummary(typeof(ShrinkZone).GetProperty(nameof(ShrinkZone.NameJa))!);
        Assert.Equal([expected!], comment);
    }
}
