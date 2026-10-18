using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace NightreignTools.Master.Tests.Integration;

/// <summary>
/// 管理用の API(マスタの管理 4.2)。管理用の鍵は Gateway が確かめるので、ここではサービス間キーだけを付けて呼ぶ。
/// ほかのテストは初期データの件数を見るので、足した行はテストの終わりに消す。
/// 行を消す API はないので、テスト用のデータベースへ直に DELETE を流す。
/// </summary>
public class AdminEndpointsTests(MasterServiceFactory factory) : IClassFixture<MasterServiceFactory>
{
    [Fact]
    public async Task AC_ADM_01_収縮候補地を登録すると201が返りプレイヤー向けの一覧に入る()
    {
        var client = factory.CreateClientWithServiceKey();

        var response = await client.PostAsJsonAsync("/api/v1/admin/shrink-zones",
            new { nameJa = "最北", centerX = 583, centerY = 297, radius = 150 });

        var created = await ReadAsync(response);
        var id = created.GetProperty("id").GetInt32();
        try
        {
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.True(id > 1000, "足した行の識別子は 1001 から振る");
            Assert.False(created.GetProperty("isHidden").GetBoolean());
            var zones = await GetArrayAsync(client, "/api/v1/shrink-zones");
            Assert.Contains(zones, z => z.GetProperty("id").GetInt32() == id && z.GetProperty("radius").GetDecimal() == 150m);
        }
        finally
        {
            await DeleteAsync("shrink_zones", id);
        }
    }

    [Fact]
    public async Task AC_ADM_02_xに1025を入れて開始地点を登録すると400が返り行が増えない()
    {
        var client = factory.CreateClientWithServiceKey();
        var before = (await GetArrayAsync(client, "/api/v1/admin/spawn-points")).Count;

        var response = await client.PostAsJsonAsync("/api/v1/admin/spawn-points", new { nameJa = "北東の教会", x = 1025, y = 100 });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("0 から 1024 の数を入れてください", (await ReadAsync(response)).GetProperty("message").GetString());
        Assert.Equal(before, (await GetArrayAsync(client, "/api/v1/admin/spawn-points")).Count);
    }

    [Fact]
    public async Task AC_ADM_03_存在しないidを修正すると404が返る()
    {
        var client = factory.CreateClientWithServiceKey();

        var response = await client.PutAsJsonAsync("/api/v1/admin/spawn-points/9999", new { nameJa = "北東の教会", x = 1, y = 1 });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("開始地点 '9999' は存在しません", (await ReadAsync(response)).GetProperty("message").GetString());
    }

    [Fact]
    public async Task AC_ADM_04_第二収縮地点を非表示にするとプレイヤー向けから消え管理用には非表示で残る()
    {
        var client = factory.CreateClientWithServiceKey();
        try
        {
            var current = await FindAsync(client, "/api/v1/admin/second-shrink-points", 12);
            var response = await client.PutAsJsonAsync("/api/v1/admin/second-shrink-points/12",
                new { nameJa = current.GetProperty("nameJa").GetString(), x = current.GetProperty("x").GetDecimal(), y = current.GetProperty("y").GetDecimal(), isHidden = true });

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.DoesNotContain(await GetArrayAsync(client, "/api/v1/second-shrink-points"), p => p.GetProperty("id").GetInt32() == 12);
            var admin = await GetArrayAsync(client, "/api/v1/admin/second-shrink-points");
            Assert.True(admin.Single(p => p.GetProperty("id").GetInt32() == 12).GetProperty("isHidden").GetBoolean());
        }
        finally
        {
            await RestoreSeedAsync("second_shrink_points", 12);
        }
    }

    [Fact]
    public async Task AC_ADM_05_非表示の開始地点を表示に戻すとプレイヤー向けの一覧に戻る()
    {
        var client = factory.CreateClientWithServiceKey();
        try
        {
            await TestDatabase.QueryAsync("UPDATE game.spawn_points SET hidden_at = now() WHERE id = 9 RETURNING id");

            var current = await FindAsync(client, "/api/v1/admin/spawn-points", 9);
            var response = await client.PutAsJsonAsync("/api/v1/admin/spawn-points/9",
                new { nameJa = current.GetProperty("nameJa").GetString(), x = current.GetProperty("x").GetDecimal(), y = current.GetProperty("y").GetDecimal(), isHidden = false });

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains(await GetArrayAsync(client, "/api/v1/spawn-points"), s => s.GetProperty("id").GetInt32() == 9);
        }
        finally
        {
            await RestoreSeedAsync("spawn_points", 9);
        }
    }

    [Fact]
    public async Task AC_ADM_06_craterの拠点候補地を登録するとmapId_craterの一覧にだけ入る()
    {
        var client = factory.CreateClientWithServiceKey();

        var created = await ReadAsync(await client.PostAsJsonAsync("/api/v1/admin/base-points", new { mapId = "crater", x = 100, y = 200 }));
        var id = created.GetProperty("id").GetInt32();
        try
        {
            var crater = await GetArrayAsync(client, "/api/v1/base-points?mapId=crater");
            Assert.Equal([id], crater.Select(b => b.GetProperty("id").GetInt32()));
            Assert.DoesNotContain(await GetArrayAsync(client, "/api/v1/base-points"), b => b.GetProperty("id").GetInt32() == id);
        }
        finally
        {
            await DeleteAsync("base_points", id);
        }
    }

    [Fact]
    public async Task AC_ADM_14_DELETEを送ると405か404が返り行が残る()
    {
        var client = factory.CreateClientWithServiceKey();

        var response = await client.DeleteAsync("/api/v1/admin/spawn-points/1");

        Assert.Contains(response.StatusCode, new[] { HttpStatusCode.MethodNotAllowed, HttpStatusCode.NotFound });
        Assert.Contains(await GetArrayAsync(client, "/api/v1/admin/spawn-points"), s => s.GetProperty("id").GetInt32() == 1);
    }

    [Fact]
    public async Task 管理用の一覧は非表示の行も含めてisHiddenを返す()
    {
        var client = factory.CreateClientWithServiceKey();

        var spawns = await GetArrayAsync(client, "/api/v1/admin/spawn-points");

        Assert.Equal(9, spawns.Count);
        Assert.Equal(["id", "nameJa", "x", "y", "isHidden"], spawns[0].EnumerateObject().Select(p => p.Name).ToArray());
    }

    [Fact]
    public async Task 必須の座標を省くと400が返る()
    {
        var client = factory.CreateClientWithServiceKey();

        var response = await client.PostAsJsonAsync("/api/v1/admin/second-shrink-points", new { nameJa = "最北", y = 1 });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("x は必須です", (await ReadAsync(response)).GetProperty("message").GetString());
    }

    private static async Task<JsonElement> ReadAsync(HttpResponseMessage response)
    {
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return document.RootElement.Clone();
    }

    private static async Task<List<JsonElement>> GetArrayAsync(HttpClient client, string path)
    {
        var response = await client.GetAsync(path);
        response.EnsureSuccessStatusCode();
        return (await ReadAsync(response)).EnumerateArray().ToList();
    }

    private static Task DeleteAsync(string table, int id) =>
        TestDatabase.QueryAsync($"DELETE FROM game.{table} WHERE id = {id} RETURNING id");

    /// <summary>管理用の一覧から 1 行を取る。初期データの行を、今の値のまま直すために使う</summary>
    private static async Task<JsonElement> FindAsync(HttpClient client, string path, int id) =>
        (await GetArrayAsync(client, path)).Single(p => p.GetProperty("id").GetInt32() == id);

    /// <summary>初期データの行を表示中に戻す。値は今のまま送っているので変わっていない</summary>
    private static Task RestoreSeedAsync(string table, int id) =>
        TestDatabase.QueryAsync($"UPDATE game.{table} SET hidden_at = NULL WHERE id = {id} RETURNING id");
}
