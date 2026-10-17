using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using NightreignTools.Record.Application.Masters;
using NightreignTools.ServiceDefaults.Security;

namespace NightreignTools.Record.Tests.Integration;

/// <summary>
/// プレイ記録の保存の API(プレイ記録 4 章、7 章)。master-service は偽物に差し替え、データベースは本物を使う。
/// テストは 1 つのデータベースを共有するので、行の数は前後の差で確かめる。
/// </summary>
public class PlayRecordEndpointsTests : IClassFixture<PlayRecordEndpointsTests.Factory>, IDisposable
{
    private readonly Factory _factory;
    private readonly HttpClient _client;

    public PlayRecordEndpointsTests(Factory factory)
    {
        _factory = factory;
        _factory.Masters.HiddenZoneIds.Clear();
        _factory.Masters.Unavailable = false;
        _client = factory.CreateClient();
        _client.DefaultRequestHeaders.Add(ServiceKeyMiddleware.HeaderName, RecordServiceFactory.ServiceKey);
    }

    public void Dispose() => _client.Dispose();

    /// <summary>正しい入力。1 日目の第 1 次と第 2 次、2 日目の第 1 次を持つ。</summary>
    private static Dictionary<string, object?> ValidRequest() => new()
    {
        ["bossId"] = "gladius",
        ["jihenId"] = null,
        ["spawnPointId"] = 1,
        ["day1NightBossId"] = 3,
        ["day2NightBossId"] = null,
        ["note"] = "2 日目は地変の方へ寄った",
        ["entries"] = new object[]
        {
            new { day = 1, phase = 1, zoneId = 4, secondShrinkPointId = (int?)null },
            new { day = 1, phase = 2, zoneId = (int?)null, secondShrinkPointId = 4 },
            new { day = 2, phase = 1, zoneId = 7, secondShrinkPointId = (int?)null },
        },
    };

    private static Dictionary<string, object?> With(string key, object? value)
    {
        var request = ValidRequest();
        request[key] = value;
        return request;
    }

    private Task<HttpResponseMessage> PostAsync(object request) => _client.PostAsJsonAsync("/api/v1/play-records", request);

    private static async Task<int> CountAsync(string table) =>
        int.Parse((await TestDatabase.QueryAsync($"SELECT count(*) FROM game.{table}"))[0]);

    private static async Task<string> MessageAsync(HttpResponseMessage response)
    {
        using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return body.RootElement.GetProperty("message").GetString()!;
    }

    /// <summary>保存されずに、指定した状態と文言が返ることを確かめる。</summary>
    private async Task AssertRejectedAsync(object request, HttpStatusCode status, string? message = null)
    {
        var before = await CountAsync("play_records");

        var response = await PostAsync(request);

        Assert.Equal(status, response.StatusCode);
        if (message is not null)
        {
            Assert.Equal(message, await MessageAsync(response));
        }

        Assert.Equal(before, await CountAsync("play_records"));
    }

    [Fact]
    public async Task AC_REC_01_正しい入力で保存すると201と識別子と日時が返り記録と収縮先の行が入る()
    {
        var records = await CountAsync("play_records");
        var entries = await CountAsync("shrink_entries");

        var response = await PostAsync(ValidRequest());

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.True(body.RootElement.GetProperty("id").GetInt32() > 0);
        Assert.EndsWith("Z", body.RootElement.GetProperty("createdAt").GetString());
        Assert.Equal(records + 1, await CountAsync("play_records"));
        Assert.Equal(entries + 3, await CountAsync("shrink_entries"));
    }

    [Fact]
    public async Task AC_REC_02_収縮先が空だと400で保存しない() =>
        await AssertRejectedAsync(With("entries", Array.Empty<object>()), HttpStatusCode.BadRequest, "entries は 1 件以上 4 件以下です");

    [Fact]
    public async Task AC_REC_03_日と段階の組が重複すると400() =>
        await AssertRejectedAsync(
            With("entries", new object[] { new { day = 1, phase = 1, zoneId = 4 }, new { day = 1, phase = 1, zoneId = 7 } }),
            HttpStatusCode.BadRequest, "day=1, phase=1 が重複しています");

    [Fact]
    public async Task AC_REC_04_存在しない収縮候補地は404で保存しない() =>
        await AssertRejectedAsync(With("entries", new object[] { new { day = 1, phase = 1, zoneId = 99 } }),
            HttpStatusCode.NotFound, "zoneId '99' は存在しません");

    [Fact]
    public async Task AC_REC_05_存在しない地変は404() =>
        await AssertRejectedAsync(With("jihenId", "unknown"), HttpStatusCode.NotFound, "jihenId 'unknown' は存在しません");

    [Fact]
    public async Task AC_REC_06_501文字の備考は400() =>
        await AssertRejectedAsync(With("note", new string('あ', 501)), HttpStatusCode.BadRequest, "note は 500 文字以内です");

    [Fact]
    public async Task AC_REC_07_空白だけの備考はNULLで保存する()
    {
        var response = await PostAsync(With("note", "   "));

        var id = await IdAsync(response);
        Assert.Equal([""], await TestDatabase.QueryAsync($"SELECT note FROM game.play_records WHERE id = {id}"));
    }

    [Fact]
    public async Task AC_REC_16_第2次を収縮候補地で送ると400() =>
        await AssertRejectedAsync(With("entries", new object[] { new { day = 1, phase = 2, zoneId = 4 } }),
            HttpStatusCode.BadRequest, "day=1, phase=2 は secondShrinkPointId で指定してください");

    [Fact]
    public async Task AC_REC_17_第1次を第二収縮地点で送ると400() =>
        await AssertRejectedAsync(With("entries", new object[] { new { day = 1, phase = 1, secondShrinkPointId = 4 } }),
            HttpStatusCode.BadRequest, "day=1, phase=1 は zoneId で指定してください");

    [Fact]
    public async Task AC_REC_18_存在しない第二収縮地点は404で保存しない() =>
        await AssertRejectedAsync(With("entries", new object[] { new { day = 1, phase = 2, secondShrinkPointId = 99 } }),
            HttpStatusCode.NotFound, "secondShrinkPointId '99' は存在しません");

    [Fact]
    public async Task AC_REC_19_1日目に2日目の夜ボスを指定すると400() =>
        await AssertRejectedAsync(With("day1NightBossId", 5), HttpStatusCode.BadRequest, "day1NightBossId には 1 日目の夜ボスを指定してください");

    [Fact]
    public async Task AC_REC_20_2日目の夜ボスを省くとNULLで保存する()
    {
        var id = await IdAsync(await PostAsync(ValidRequest()));

        var row = await TestDatabase.QueryAsync(
            $"SELECT coalesce(day1_night_boss_id::text, 'NULL') || ',' || coalesce(day2_night_boss_id::text, 'NULL') FROM game.play_records WHERE id = {id}");
        Assert.Equal(["3,NULL"], row);
    }

    [Fact]
    public async Task AC_REC_23_master_serviceが応答しないと503で保存しない()
    {
        _factory.Masters.Unavailable = true;

        await AssertRejectedAsync(ValidRequest(), HttpStatusCode.ServiceUnavailable,
            "マスタのサービスに接続できません。時間をおいてやり直してください");
    }

    [Fact]
    public async Task AC_REC_24_非表示の収縮候補地は400で保存しない()
    {
        _factory.Masters.HiddenZoneIds.Add(4);

        await AssertRejectedAsync(ValidRequest(), HttpStatusCode.BadRequest, "zoneId '4' は非表示のため、記録に使えません");
    }

    [Fact]
    public async Task 開始地点を省くと400() =>
        await AssertRejectedAsync(With("spawnPointId", null), HttpStatusCode.BadRequest, "spawnPointId は必須です");

    [Fact]
    public async Task 収縮先が5件だと400() =>
        await AssertRejectedAsync(With("entries", Enumerable.Range(0, 5).Select(_ => new { day = 1, phase = 1, zoneId = 4 }).ToArray()),
            HttpStatusCode.BadRequest, "entries は 1 件以上 4 件以下です");

    [Fact]
    public async Task 記録の中の規則に反するときはmaster_serviceに問い合わせない()
    {
        var before = _factory.Masters.Queries.Count;

        await PostAsync(With("entries", new object[] { new { day = 1, phase = 2, zoneId = 4 } }));

        Assert.Equal(before, _factory.Masters.Queries.Count);
    }

    private static async Task<int> IdAsync(HttpResponseMessage response)
    {
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return body.RootElement.GetProperty("id").GetInt32();
    }

    /// <summary>master-service のクライアントを偽物に差し替えて起動する。</summary>
    public sealed class Factory : RecordServiceFactory
    {
        internal FakeMasterDirectory Masters { get; } = new();

        protected override void ConfigureTestServices(IServiceCollection services) =>
            services.AddSingleton<IMasterDirectory>(Masters);
    }
}
