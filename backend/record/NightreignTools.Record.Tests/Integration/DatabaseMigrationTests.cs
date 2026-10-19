using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NightreignTools.Record.Infrastructure.Persistence;
using Npgsql;

namespace NightreignTools.Record.Tests.Integration;

/// <summary>
/// 起動のときのマイグレーション(データベースの共通仕様 5.5)と、記録のテーブルの制約(プレイ記録 5 章)。
/// </summary>
public class DatabaseMigrationTests
{
    private static readonly string[] ExpectedTables = ["__ef_migrations_history", "play_records", "shrink_entries"];

    [Fact]
    public async Task AC_DB_01_空のデータベースで起動するとgameスキーマにrecordのテーブルがすべてでき履歴にすべてのマイグレーションが入る()
    {
        TestDatabase.Drop(TestDatabase.Name);
        using var factory = new RecordServiceFactory();
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
        using (var first = new RecordServiceFactory())
        {
            first.CreateClient();
        }
        var before = await TestDatabase.QueryAsync("SELECT migration_id FROM game.__ef_migrations_history ORDER BY migration_id");

        using var second = new RecordServiceFactory();
        var response = await second.CreateClient().GetAsync("/health");

        Assert.True(response.IsSuccessStatusCode);
        var after = await TestDatabase.QueryAsync("SELECT migration_id FROM game.__ef_migrations_history ORDER BY migration_id");
        Assert.Equal(before, after);
    }

    [Theory]
    // 第 1 次に第二収縮地点を持たせる
    [InlineData(1, "NULL", "4")]
    // 第 2 次に収縮候補地を持たせる
    [InlineData(2, "4", "NULL")]
    // 収縮先がない
    [InlineData(1, "NULL", "NULL")]
    public async Task 段階と収縮先の種類が合わない行はデータベースの制約で入らない(int phase, string zoneId, string secondShrinkPointId)
    {
        using var factory = new RecordServiceFactory();
        factory.CreateClient();
        var recordId = await InsertPlayRecordAsync();

        var exception = await Assert.ThrowsAsync<PostgresException>(() => TestDatabase.ExecuteAsync(
            $"INSERT INTO game.shrink_entries (play_record_id, day, phase, zone_id, second_shrink_point_id) VALUES ({recordId}, 1, {phase}, {zoneId}, {secondShrinkPointId})"));

        Assert.Equal(PostgresErrorCodes.CheckViolation, exception.SqlState);
    }

    [Fact]
    public async Task 同じ記録で日と段階の組が重複する行はデータベースの制約で入らない()
    {
        using var factory = new RecordServiceFactory();
        factory.CreateClient();
        var recordId = await InsertPlayRecordAsync();
        var insert = $"INSERT INTO game.shrink_entries (play_record_id, day, phase, zone_id) VALUES ({recordId}, 1, 1, 4)";
        await TestDatabase.ExecuteAsync(insert);

        var exception = await Assert.ThrowsAsync<PostgresException>(() => TestDatabase.ExecuteAsync(insert));

        Assert.Equal(PostgresErrorCodes.UniqueViolation, exception.SqlState);
    }

    /// <summary>
    /// このサービスの持つマイグレーションの一覧(古い順)。
    /// </summary>
    /// <param name="factory">テストのために起動した record-service。</param>
    /// <returns>マイグレーションの名前。古い順。</returns>
    private static List<string> AllMigrations(RecordServiceFactory factory)
    {
        using var scope = factory.Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<RecordDbContext>().Database.GetMigrations().Order().ToList();
    }

    /// <summary>
    /// 収縮先を持たない記録を 1 行入れ、その識別子を返す。
    /// </summary>
    /// <returns>入れた記録の識別子。</returns>
    private static async Task<string> InsertPlayRecordAsync()
    {
        var ids = await TestDatabase.QueryAsync(
            "INSERT INTO game.play_records (boss_id, spawn_point_id) VALUES ('gladius', 1) RETURNING id");
        return ids[0];
    }
}
