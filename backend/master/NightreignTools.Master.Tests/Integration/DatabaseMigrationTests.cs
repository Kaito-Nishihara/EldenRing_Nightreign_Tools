using Npgsql;

namespace NightreignTools.Master.Tests.Integration;

/// <summary>
/// 起動のときのマイグレーション(データベースの共通仕様 5.5)。
/// </summary>
public class DatabaseMigrationTests
{
    private static readonly string[] ExpectedTables =
    [
        "__ef_migrations_history",
        "base_points",
        "boss_weaknesses",
        "bosses",
        "jihen",
        "night_bosses",
        "second_shrink_points",
        "shrink_zones",
        "spawn_points",
    ];

    [Fact]
    public async Task AC_DB_01_空のデータベースで起動するとgameスキーマにmasterのテーブルがすべてでき履歴に1行入る()
    {
        TestDatabase.Drop(TestDatabase.Name);
        using var factory = new MasterServiceFactory();
        factory.CreateClient();

        var tables = await TestDatabase.QueryAsync(
            "SELECT table_name FROM information_schema.tables WHERE table_schema = 'game' ORDER BY table_name");
        Assert.Equal(ExpectedTables, tables);

        var history = await TestDatabase.QueryAsync("SELECT migration_id FROM game.__ef_migrations_history");
        Assert.Single(history);
        Assert.EndsWith("_InitialCreate", history[0]);
    }

    [Fact]
    public async Task AC_DB_02_同じデータベースで2回起動しても2回目は適用せずに起動が成功する()
    {
        using (var first = new MasterServiceFactory())
        {
            first.CreateClient();
        }

        using var second = new MasterServiceFactory();
        var response = await second.CreateClient().GetAsync("/health");

        Assert.True(response.IsSuccessStatusCode);
        var history = await TestDatabase.QueryAsync("SELECT migration_id FROM game.__ef_migrations_history");
        Assert.Single(history);
    }

    [Fact]
    public void AC_DB_08_自分の利用者がないサーバーにつなぐと起動が止まりボリュームを消す手順が出る()
    {
        // 旧版のボリュームが残っていると、db/init/ が走らず、サービスの利用者がない
        var missingUser = new NpgsqlConnectionStringBuilder(TestDatabase.ConnectionString)
        {
            Username = "nightreign_missing_user",
            Password = "wrong",
        }.ToString();
        using var factory = new FactoryWithConnectionString(missingUser);

        var exception = Record.Exception(() => factory.CreateClient());

        Assert.NotNull(exception);
        Assert.Contains("db/init/", exception.ToString());
        Assert.Contains("docker volume rm", exception.ToString());
    }

    private sealed class FactoryWithConnectionString(string connectionString) : MasterServiceFactory
    {
        protected override string ConfiguredConnectionString => connectionString;
    }
}
