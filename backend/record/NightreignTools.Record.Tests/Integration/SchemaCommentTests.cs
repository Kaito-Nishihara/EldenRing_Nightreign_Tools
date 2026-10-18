using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using NightreignTools.Record.Domain;
using NightreignTools.Record.Domain.Entities;
using NightreignTools.Record.Infrastructure.Persistence;
using NightreignTools.ServiceDefaults.Persistence;

namespace NightreignTools.Record.Tests.Integration;

/// <summary>
/// テーブルと列の説明(データベースの共通仕様 5.8)。説明はエンティティの &lt;summary&gt; から取る。
/// </summary>
public class SchemaCommentTests
{
    [Fact]
    public void AC_DB_10_recordのすべてのテーブルとすべての列に説明がある()
    {
        using var context = new DesignTimeDbContextFactory().CreateDbContext([]);
        var model = context.GetService<IDesignTimeModel>().Model;

        Assert.Empty(DocumentationCommentExtensions.FindMissingComments(model));
    }

    [Fact]
    public async Task AC_DB_11_マイグレーションを適用したデータベースのテーブルのコメントがエンティティの説明と同じである()
    {
        using var factory = new RecordServiceFactory();
        factory.CreateClient();

        var comment = await TestDatabase.QueryAsync("SELECT obj_description('game.play_records'::regclass, 'pg_class')");

        var expected = XmlDocumentation.Load(typeof(DomainAssembly).Assembly).GetSummary(typeof(PlayRecord));
        Assert.Equal([expected!], comment);
    }
}
