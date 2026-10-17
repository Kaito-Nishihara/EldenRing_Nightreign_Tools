using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace NightreignTools.Master.Tests.Integration;

/// <summary>
/// マスタの参照の解決の内部の API(マスタの管理 4.4)。
/// </summary>
public class ReferenceEndpointsTests(MasterServiceFactory factory) : IClassFixture<MasterServiceFactory>
{
    private const string Path = "/internal/v1/references/resolve";

    [Fact]
    public async Task AC_ADM_18_ない識別子は抜いて見つかった行だけを返す()
    {
        var body = await ResolveAsync(new { zoneIds = new[] { 4, 99 } });

        var zones = body.GetProperty("zones").EnumerateArray().ToList();
        Assert.Single(zones);
        Assert.Equal(4, zones[0].GetProperty("id").GetInt32());
        Assert.Equal("最北", zones[0].GetProperty("nameJa").GetString());
        Assert.Equal(255m, zones[0].GetProperty("radius").GetDecimal());
        Assert.False(zones[0].GetProperty("isHidden").GetBoolean());
    }

    [Fact]
    public async Task AC_ADM_19_非表示の収縮候補地もisHiddenがtrueで返る()
    {
        factory.CreateClient();
        await TestDatabase.QueryAsync("UPDATE game.shrink_zones SET hidden_at = now() WHERE id = 7 RETURNING id");
        try
        {
            var body = await ResolveAsync(new { zoneIds = new[] { 7 } });

            var zone = body.GetProperty("zones").EnumerateArray().Single();
            Assert.True(zone.GetProperty("isHidden").GetBoolean());
        }
        finally
        {
            await TestDatabase.QueryAsync("UPDATE game.shrink_zones SET hidden_at = NULL WHERE id = 7 RETURNING id");
        }
    }

    [Fact]
    public async Task 夜の王と地変と開始地点と第二収縮地点と夜ボスを1回で引ける()
    {
        var body = await ResolveAsync(new
        {
            bossIds = new[] { "gladius", "unknown" },
            jihenIds = new[] { "crater" },
            spawnPointIds = new[] { 1 },
            secondShrinkPointIds = new[] { 4, 4 },
            nightBossIds = new[] { 3, 20 },
        });

        Assert.Equal("三つ首の獣", body.GetProperty("bosses").EnumerateArray().Single().GetProperty("nameJa").GetString());
        Assert.Equal("火口", body.GetProperty("jihen").EnumerateArray().Single().GetProperty("nameJa").GetString());
        Assert.Equal(351m, body.GetProperty("spawnPoints").EnumerateArray().Single().GetProperty("x").GetDecimal());
        // 同じ識別子を 2 回渡しても、1 行だけ返る
        Assert.Single(body.GetProperty("secondShrinkPoints").EnumerateArray());
        Assert.Equal([1, 2], body.GetProperty("nightBosses").EnumerateArray().Select(n => n.GetProperty("day").GetInt32()).ToArray());
    }

    [Fact]
    public async Task 配列を省くと空の配列が返る()
    {
        var body = await ResolveAsync(new { });

        foreach (var name in new[] { "bosses", "jihen", "spawnPoints", "zones", "secondShrinkPoints", "nightBosses" })
        {
            Assert.Empty(body.GetProperty(name).EnumerateArray());
        }
    }

    [Fact]
    public async Task 配列が100件を超えると400がエラーの形で返る()
    {
        var response = await factory.CreateClientWithServiceKey()
            .PostAsJsonAsync(Path, new { zoneIds = Enumerable.Range(1, 101).ToArray() });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var root = document.RootElement;
        Assert.Equal(["error", "message", "status", "timestamp"], root.EnumerateObject().Select(p => p.Name).Order().ToArray());
        Assert.Equal("zoneIds は 100 件までです", root.GetProperty("message").GetString());
    }

    [Fact]
    public async Task キーなしで呼ぶと401が返る()
    {
        var response = await factory.CreateClient().PostAsJsonAsync(Path, new { zoneIds = new[] { 1 } });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private async Task<JsonElement> ResolveAsync(object request)
    {
        var response = await factory.CreateClientWithServiceKey().PostAsJsonAsync(Path, request);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return document.RootElement.Clone();
    }
}
