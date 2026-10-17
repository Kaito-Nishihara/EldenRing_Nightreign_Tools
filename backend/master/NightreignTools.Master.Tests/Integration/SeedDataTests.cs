using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NightreignTools.Master.Domain.Entities;
using NightreignTools.Master.Infrastructure.Persistence;
using NightreignTools.Master.Infrastructure.Persistence.Seed;

namespace NightreignTools.Master.Tests.Integration;

/// <summary>
/// マイグレーションに含めた初期データと、テーブルの制約(データベースの共通仕様 5.3、5.6)。
/// API から見た初期データ(AC-DB-03〜05)は、マスタの API のテストで確かめる。
/// </summary>
public class SeedDataTests(MasterServiceFactory factory) : IClassFixture<MasterServiceFactory>
{
    [Fact]
    public async Task 初期データの件数が旧版のマイグレーションと一致する()
    {
        await using var scope = CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<MasterDbContext>();

        Assert.Equal(10, await db.Bosses.CountAsync());
        Assert.Equal(15, await db.BossWeaknesses.CountAsync());
        Assert.Equal(5, await db.Jihen.CountAsync());
        Assert.Equal(12, await db.ShrinkZones.CountAsync());
        Assert.Equal(12, await db.SecondShrinkPoints.CountAsync());
        Assert.Equal(9, await db.SpawnPoints.CountAsync());
        Assert.Equal(15, await db.BasePoints.CountAsync(b => b.MapId == BasePoint.NormalMapId));
        Assert.Equal(35, await db.NightBosses.CountAsync());
        Assert.Equal(14, await db.NightBosses.CountAsync(n => n.Day == 1));
    }

    [Fact]
    public async Task 地変の中心は1024四方の座標空間の数に直してある()
    {
        await using var scope = CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<MasterDbContext>();

        var crater = await db.Jihen.SingleAsync(j => j.Id == "crater");

        Assert.Equal(512m, crater.CenterX);
        Assert.Equal(204.8m, crater.CenterY);
    }

    [Fact]
    public async Task 収縮候補地の初期データは半径255で表示中である()
    {
        await using var scope = CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<MasterDbContext>();

        var zones = await db.ShrinkZones.ToListAsync();

        Assert.All(zones, z => Assert.Equal(MapPlaceSeed.ZoneRadius, z.Radius));
        Assert.All(zones, z => Assert.False(z.IsHidden));
    }

    [Fact]
    public async Task 画面から足した行の識別子は初期データの後の1001から振られる()
    {
        factory.CreateClient();

        var ids = await TestDatabase.QueryAsync(
            "INSERT INTO game.spawn_points (name_ja, x, y) VALUES ('テストの開始地点', 100, 100) RETURNING id");
        await TestDatabase.QueryAsync($"DELETE FROM game.spawn_points WHERE id = {ids[0]} RETURNING id");

        Assert.True(int.Parse(ids[0], System.Globalization.CultureInfo.InvariantCulture) > SeedData.IdLimit);
    }

    [Fact]
    public async Task 座標が1024を超える行は検査の制約で拒まれる()
    {
        factory.CreateClient();

        var exception = await Record.ExceptionAsync(() => TestDatabase.QueryAsync(
            "INSERT INTO game.spawn_points (name_ja, x, y) VALUES ('範囲の外', 1025, 100) RETURNING id"));

        Assert.NotNull(exception);
        Assert.Contains("ck_spawn_points_x", exception.Message);
    }

    /// <summary>起動(とマイグレーション)を済ませてから、アプリと同じ設定の DbContext を持つスコープを作る。</summary>
    private AsyncServiceScope CreateScope()
    {
        factory.CreateClient();
        return factory.Services.CreateAsyncScope();
    }
}
