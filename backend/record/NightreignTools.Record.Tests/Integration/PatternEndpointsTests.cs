using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using NightreignTools.Record.Application.Masters;
using NightreignTools.ServiceDefaults.Security;

namespace NightreignTools.Record.Tests.Integration;

/// <summary>
/// パターン検索の API(パターン検索 4 章、7 章)。master-service は偽物に差し替え、データベースは本物を使う。
/// 記録は保存の API で入れる。テストは 1 つのデータベースを共有するので、件数は前後の差で確かめる。
/// 開始地点 2 は、このクラスだけが使う。開始地点 3 には記録を保存しない。
/// </summary>
public class PatternEndpointsTests : IClassFixture<PatternEndpointsTests.Factory>, IDisposable
{
    private readonly Factory _factory;
    private readonly HttpClient _client;

    public PatternEndpointsTests(Factory factory)
    {
        _factory = factory;
        _factory.Masters.Unavailable = false;
        _client = factory.CreateClient();
        _client.DefaultRequestHeaders.Add(ServiceKeyMiddleware.HeaderName, RecordServiceFactory.ServiceKey);
    }

    public void Dispose() => _client.Dispose();

    private async Task SaveAsync(int spawnPointId, string? jihenId, params object[] entries)
    {
        var response = await _client.PostAsJsonAsync("/api/v1/play-records",
            new { bossId = "gladius", spawnPointId, jihenId, entries });
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    private async Task<JsonElement> SearchAsync(string query)
    {
        var response = await _client.GetAsync($"/api/v1/patterns?{query}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return body.RootElement.Clone();
    }

    private async Task<int> TotalAsync(string query) => (await SearchAsync(query)).GetProperty("totalRecords").GetInt32();

    [Fact]
    public async Task AC_REC_08_保存した直後に同じ条件で検索すると件数が1増える()
    {
        var before = await TotalAsync("bossId=gladius&spawnPointId=2");

        await SaveAsync(2, null, new { day = 1, phase = 1, zoneId = 4 });

        Assert.Equal(before + 1, await TotalAsync("bossId=gladius&spawnPointId=2"));
    }

    [Fact]
    public async Task AC_PAT_04_地変を省いて検索すると地変ありの記録が数に入らない()
    {
        var before = await TotalAsync("bossId=gladius&spawnPointId=2");

        await SaveAsync(2, "crater", new { day = 1, phase = 1, zoneId = 4 });

        Assert.Equal(before, await TotalAsync("bossId=gladius&spawnPointId=2"));
    }

    [Fact]
    public async Task AC_PAT_05_地変を指定して検索すると地変なしの記録が数に入らない()
    {
        var before = await TotalAsync("bossId=gladius&spawnPointId=2&jihenId=crater");

        await SaveAsync(2, null, new { day = 1, phase = 1, zoneId = 7 });

        Assert.Equal(before, await TotalAsync("bossId=gladius&spawnPointId=2&jihenId=crater"));
    }

    [Fact]
    public async Task AC_PAT_06_記録のない条件では200と0件と空の一覧が返る()
    {
        var result = await SearchAsync("bossId=gladius&spawnPointId=3");

        Assert.Equal(0, result.GetProperty("totalRecords").GetInt32());
        Assert.Equal(0, result.GetProperty("patterns").GetArrayLength());
    }

    [Fact]
    public async Task AC_PAT_16_第1次はzoneに第2次はsecondShrinkPointに入りもう一方はnullで返る()
    {
        await SaveAsync(2, "crater",
            new { day = 1, phase = 1, zoneId = 4 },
            new { day = 1, phase = 2, secondShrinkPointId = 4 });

        var pattern = (await SearchAsync("bossId=gladius&spawnPointId=2&jihenId=crater")).GetProperty("patterns")
            .EnumerateArray()
            .First(p => p.GetProperty("entries").GetArrayLength() == 2);
        var entries = pattern.GetProperty("entries");

        Assert.Equal(4, entries[0].GetProperty("zone").GetProperty("id").GetInt32());
        Assert.Equal(255m, entries[0].GetProperty("zone").GetProperty("radius").GetDecimal());
        Assert.Equal(JsonValueKind.Null, entries[0].GetProperty("secondShrinkPoint").ValueKind);
        Assert.Equal(JsonValueKind.Null, entries[1].GetProperty("zone").ValueKind);
        Assert.Equal("最北", entries[1].GetProperty("secondShrinkPoint").GetProperty("nameJa").GetString());
        Assert.EndsWith("Z", pattern.GetProperty("lastRecordedAt").GetString());
    }

    [Theory]
    [InlineData("bossId=gladius", "spawnPointId は必須です")]
    [InlineData("spawnPointId=1", "bossId は必須です")]
    public async Task AC_PAT_08_夜の王か開始地点を省くと400(string query, string message)
    {
        var response = await _client.GetAsync($"/api/v1/patterns?{query}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal(message, body.RootElement.GetProperty("message").GetString());
    }

    [Fact]
    public async Task 開始地点が整数でないと400()
    {
        var response = await _client.GetAsync("/api/v1/patterns?bossId=gladius&spawnPointId=abc");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task AC_PAT_09_存在しない夜の王で検索すると404()
    {
        var response = await _client.GetAsync("/api/v1/patterns?bossId=unknown&spawnPointId=1");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AC_PAT_20_master_serviceが応答しないと503()
    {
        _factory.Masters.Unavailable = true;

        var response = await _client.GetAsync("/api/v1/patterns?bossId=gladius&spawnPointId=1");

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }

    /// <summary>
    /// master-service のクライアントを偽物に差し替えて起動する。
    /// </summary>
    public sealed class Factory : RecordServiceFactory
    {
        internal FakeMasterDirectory Masters { get; } = new();

        protected override void ConfigureTestServices(IServiceCollection services) =>
            services.AddSingleton<IMasterDirectory>(Masters);
    }
}
