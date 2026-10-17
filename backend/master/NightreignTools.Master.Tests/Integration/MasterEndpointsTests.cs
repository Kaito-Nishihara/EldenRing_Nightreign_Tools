using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace NightreignTools.Master.Tests.Integration;

/// <summary>
/// プレイヤー向けのマスタの取得の API(マスタの管理 4.1、データベースの共通仕様 7 章)。
/// </summary>
public class MasterEndpointsTests(MasterServiceFactory factory) : IClassFixture<MasterServiceFactory>
{
    [Fact]
    public async Task AC_DB_03_bossesは10件を返しgladiusの弱点はholyの1件である()
    {
        var bosses = await GetArrayAsync("/api/v1/bosses");

        Assert.Equal(10, bosses.Count);
        var gladius = bosses.Single(b => b.GetProperty("id").GetString() == "gladius");
        Assert.Equal("三つ首の獣", gladius.GetProperty("nameJa").GetString());
        Assert.Equal(["holy"], gladius.GetProperty("weaknesses").EnumerateArray().Select(w => w.GetString() ?? "").ToArray());
    }

    [Fact]
    public async Task AC_ADM_23_夜の王と地変は初期データの並び順で返りDLCのものが最後に来る()
    {
        var bosses = await GetArrayAsync("/api/v1/bosses");
        var jihen = await GetArrayAsync("/api/v1/jihen");

        Assert.Equal(
            ["gladius", "edere", "gnostor", "maris", "libra", "fulgor", "caligo", "nameless", "harmonia", "strages"],
            bosses.Select(b => b.GetProperty("id").GetString() ?? "").ToArray());
        Assert.Equal(
            ["crater", "mountain", "rotted_forest", "noklateo", "great_cavern"],
            jihen.Select(j => j.GetProperty("id").GetString() ?? "").ToArray());
        Assert.True(bosses[^1].GetProperty("isDlc").GetBoolean());
        Assert.True(jihen[^1].GetProperty("isDlc").GetBoolean());
    }

    [Fact]
    public async Task AC_DB_04_night_bossesは35件を返しdayが1のものは14件である()
    {
        var nightBosses = await GetArrayAsync("/api/v1/night-bosses");

        Assert.Equal(35, nightBosses.Count);
        Assert.Equal(14, nightBosses.Count(n => n.GetProperty("day").GetInt32() == 1));
    }

    [Fact]
    public async Task AC_DB_05_jihenのcraterの中心は1024四方の座標空間の数である()
    {
        var jihen = await GetArrayAsync("/api/v1/jihen");

        var crater = jihen.Single(j => j.GetProperty("id").GetString() == "crater");
        Assert.Equal(512m, crater.GetProperty("centerX").GetDecimal());
        Assert.Equal(204.8m, crater.GetProperty("centerY").GetDecimal());
    }

    [Fact]
    public async Task 開始地点と収縮候補地と第二収縮地点は識別子の順に返る()
    {
        var spawns = await GetArrayAsync("/api/v1/spawn-points");
        var zones = await GetArrayAsync("/api/v1/shrink-zones");
        var points = await GetArrayAsync("/api/v1/second-shrink-points");

        Assert.Equal(9, spawns.Count);
        Assert.Equal(12, zones.Count);
        Assert.Equal(12, points.Count);
        Assert.Equal(Enumerable.Range(1, 12), zones.Select(z => z.GetProperty("id").GetInt32()));
        Assert.Equal(["id", "nameJa", "iconPath", "centerX", "centerY", "radius"], zones[0].EnumerateObject().Select(p => p.Name).ToArray());
    }

    [Fact]
    public async Task 拠点候補地はmapIdを省くと通常のマップの15件を返す()
    {
        var bases = await GetArrayAsync("/api/v1/base-points");

        Assert.Equal(15, bases.Count);
        Assert.All(bases, b => Assert.Equal("normal", b.GetProperty("mapId").GetString()));
    }

    [Fact]
    public async Task AC_ADM_06_mapIdにcraterを渡すとcraterの行だけを返す()
    {
        // 火口の拠点候補地は初期データにないので、まだ 0 件である
        var bases = await GetArrayAsync("/api/v1/base-points?mapId=crater");

        Assert.Empty(bases);
    }

    [Fact]
    public async Task AC_ADM_21_mapIdが通常でも地変でもなければ404を返す()
    {
        var response = await factory.CreateClientWithServiceKey().GetAsync("/api/v1/base-points?mapId=unknown");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal("mapId 'unknown' は存在しません", body.RootElement.GetProperty("message").GetString());
    }

    [Fact]
    public async Task AC_ADM_22_非表示の開始地点は一覧に入らない()
    {
        factory.CreateClient();
        await TestDatabase.QueryAsync("UPDATE game.spawn_points SET hidden_at = now() WHERE id = 9 RETURNING id");
        try
        {
            var spawns = await GetArrayAsync("/api/v1/spawn-points");

            Assert.Equal(8, spawns.Count);
            Assert.DoesNotContain(spawns, s => s.GetProperty("id").GetInt32() == 9);
        }
        finally
        {
            await TestDatabase.QueryAsync("UPDATE game.spawn_points SET hidden_at = NULL WHERE id = 9 RETURNING id");
        }
    }

    [Fact]
    public async Task AC_SVC_07_api_v1の下のすべてのエンドポイントはキーなしで401を返す()
    {
        var client = factory.CreateClient();
        var paths = factory.Services.GetRequiredService<EndpointDataSource>().Endpoints
            .OfType<RouteEndpoint>()
            .Select(e => "/" + e.RoutePattern.RawText!.TrimStart('/'))
            .Where(p => p.StartsWith("/api/v1", StringComparison.Ordinal))
            .Distinct()
            .ToList();

        Assert.NotEmpty(paths);
        foreach (var path in paths)
        {
            var response = await client.GetAsync(path);
            Assert.True(response.StatusCode == HttpStatusCode.Unauthorized, path + " が " + (int)response.StatusCode + " を返した");
        }
    }

    private async Task<List<JsonElement>> GetArrayAsync(string path)
    {
        var response = await factory.CreateClientWithServiceKey().GetAsync(path);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return document.RootElement.EnumerateArray().Select(e => e.Clone()).ToList();
    }
}
