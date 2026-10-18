using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using NightreignTools.Relic.Domain;
using NightreignTools.Relic.Domain.Entities;
using NightreignTools.Relic.Infrastructure.Persistence;
using NightreignTools.ServiceDefaults.Persistence;

namespace NightreignTools.Relic.Tests.Integration;

/// <summary>
/// テーブルと列の説明(データベースの共通仕様 5.8)。説明はエンティティの &lt;summary&gt; から取る。
/// </summary>
public class SchemaCommentTests
{
    [Fact]
    public void AC_DB_10_relicのすべてのテーブルとすべての列に説明がある()
    {
        using var context = new DesignTimeDbContextFactory().CreateDbContext([]);
        var model = context.GetService<IDesignTimeModel>().Model;

        Assert.Empty(DocumentationCommentExtensions.FindMissingComments(model));
    }

    [Fact]
    public async Task AC_DB_11_マイグレーションを適用したデータベースのテーブルのコメントがエンティティの説明と同じである()
    {
        using var factory = new RelicServiceFactory();
        factory.CreateClient();

        var comment = await TestDatabase.QueryAsync("SELECT obj_description('game.relic_effects'::regclass, 'pg_class')");

        var expected = XmlDocumentation.Load(typeof(DomainAssembly).Assembly).GetSummary(typeof(RelicEffect));
        Assert.Equal([expected!], comment);
    }
}
