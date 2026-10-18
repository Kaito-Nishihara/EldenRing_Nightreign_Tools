using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NightreignTools.Relic.Infrastructure.Persistence;
using Npgsql;

namespace NightreignTools.Relic.Tests.Integration;

/// <summary>
/// 起動のときのマイグレーション(データベースの共通仕様 5.5)と、遺物の効果のテーブルの制約(ビルドシミュレーター 5 章)。
/// </summary>
public class DatabaseMigrationTests
{
    private static readonly string[] ExpectedTables =
        ["__ef_migrations_history", "relic_effect_categories", "relic_effect_ranks", "relic_effects"];

    [Fact]
    public async Task AC_DB_01_空のデータベースで起動するとgameスキーマにrelicのテーブルがすべてでき履歴にすべてのマイグレーションが入る()
    {
        TestDatabase.Drop(TestDatabase.Name);
        using var factory = new RelicServiceFactory();
        factory.CreateClient();

        var tables = await TestDatabase.QueryAsync(
            "SELECT table_name FROM information_schema.tables WHERE table_schema = 'game' ORDER BY table_name");
        Assert.Equal(ExpectedTables, tables);

        var history = await TestDatabase.QueryAsync("SELECT migration_id FROM game.__ef_migrations_history ORDER BY migration_id");
        Assert.Equal(AllMigrations(factory), history);
        Assert.EndsWith("_InitialCreate", history[0]);
    }

    [Fact]
    public async Task AC_DB_02_同じデータベースで2回起動しても2回目は適用せずに起動が成功する()
    {
        using (var first = new RelicServiceFactory())
        {
            first.CreateClient();
        }
        var before = await TestDatabase.QueryAsync("SELECT migration_id FROM game.__ef_migrations_history ORDER BY migration_id");

        using var second = new RelicServiceFactory();
        var response = await second.CreateClient().GetAsync("/health");

        Assert.True(response.IsSuccessStatusCode);
        var after = await TestDatabase.QueryAsync("SELECT migration_id FROM game.__ef_migrations_history ORDER BY migration_id");
        Assert.Equal(before, after);
    }

    [Fact]
    public async Task 同じ効果で同じランクの行はデータベースの制約で入らない()
    {
        using var factory = new RelicServiceFactory();
        factory.CreateClient();

        // 効果 1(最大HP上昇)はランク 0 の行を初期データで持つ
        var exception = await Assert.ThrowsAsync<PostgresException>(() => TestDatabase.ExecuteAsync(
            "INSERT INTO game.relic_effect_ranks (effect_id, rank, value, unit) VALUES (1, 0, 50, 'flat')"));

        Assert.Equal(PostgresErrorCodes.UniqueViolation, exception.SqlState);
    }

    [Theory]
    [InlineData("relic_effects", "calc_type", "'divide'")]
    [InlineData("relic_effects", "effect_sign", "'neutral'")]
    [InlineData("relic_effects", "stackable", "'maybe'")]
    [InlineData("relic_effect_ranks", "unit", "'meter'")]
    public async Task 決まった値の一覧にない値はデータベースの制約で入らない(string table, string column, string value)
    {
        using var factory = new RelicServiceFactory();
        factory.CreateClient();

        var exception = await Assert.ThrowsAsync<PostgresException>(() => TestDatabase.ExecuteAsync(
            $"UPDATE game.{table} SET {column} = {value} WHERE id = 1"));

        Assert.Equal(PostgresErrorCodes.CheckViolation, exception.SqlState);
    }

    /// <summary>このサービスの持つマイグレーションの一覧(古い順)。</summary>
    private static List<string> AllMigrations(RelicServiceFactory factory)
    {
        using var scope = factory.Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<RelicDbContext>().Database.GetMigrations().Order().ToList();
    }
}
