using System.Net;
using Microsoft.Extensions.Logging.Abstractions;
using NightreignTools.Record.Application.Masters;
using NightreignTools.Record.Infrastructure.Masters;
using NightreignTools.SharedKernel;

namespace NightreignTools.Record.Tests.Unit;

/// <summary>
/// master-service のクライアントの入出力(サービスの共通構成 5.5、マスタの管理 4.4)。HTTP は偽物に差し替える。
/// 期限と再試行は登録のときに付くので、<see cref="NightreignTools.Record.Tests.Integration.MasterDirectoryRegistrationTests"/> で確かめる。
/// </summary>
public class MasterDirectoryClientTests
{
    private static readonly MasterReferenceQuery Query = new(["gladius"], [], [1], [4, 7], [4], [3]);

    private const string ResolvedJson = """
        {
          "bosses": [{ "id": "gladius", "nameJa": "三つ首の獣" }],
          "jihen": [],
          "spawnPoints": [{ "id": 1, "nameJa": "北東の教会", "x": 351.0, "y": 387.0, "isHidden": false }],
          "zones": [{ "id": 4, "nameJa": "最北", "centerX": 583.0, "centerY": 297.0, "radius": 255.0, "isHidden": true }],
          "secondShrinkPoints": [{ "id": 4, "nameJa": "最北", "x": 583.0, "y": 297.0, "isHidden": false }],
          "nightBosses": [{ "id": 3, "nameJa": "貪食ドラゴン", "day": 1 }]
        }
        """;

    private static MasterDirectoryClient CreateClient(StubHttpHandler handler) =>
        new(new HttpClient(handler) { BaseAddress = new Uri("http://master-service.test/") }, NullLogger<MasterDirectoryClient>.Instance);

    [Fact]
    public async Task 内部の参照の解決のAPIへ識別子をcamelCaseで送る()
    {
        var handler = StubHttpHandler.Returning(HttpStatusCode.OK, ResolvedJson);

        await CreateClient(handler).ResolveAsync(Query, CancellationToken.None);

        var request = Assert.Single(handler.Requests);
        Assert.Equal(HttpMethod.Post, request.Method);
        Assert.Equal("http://master-service.test/internal/v1/references/resolve", request.RequestUri!.ToString());
        Assert.Contains("\"bossIds\":[\"gladius\"]", handler.Bodies[0]);
        Assert.Contains("\"zoneIds\":[4,7]", handler.Bodies[0]);
        Assert.Contains("\"nightBossIds\":[3]", handler.Bodies[0]);
    }

    [Fact]
    public async Task 応答を読み非表示の行と夜ボスの日も受け取る()
    {
        var result = await CreateClient(StubHttpHandler.Returning(HttpStatusCode.OK, ResolvedJson)).ResolveAsync(Query, CancellationToken.None);

        Assert.Equal("三つ首の獣", Assert.Single(result.Bosses).NameJa);
        Assert.Empty(result.Jihen);
        var zone = Assert.Single(result.Zones);
        Assert.True(zone.IsHidden);
        Assert.Equal(255.0m, zone.Radius);
        Assert.Equal(1, Assert.Single(result.NightBosses).Day);
    }

    [Theory]
    [InlineData(HttpStatusCode.InternalServerError)]
    [InlineData(HttpStatusCode.ServiceUnavailable)]
    // サービス間キーの食い違いなどの設定の誤りも、利用者には 503 として伝える
    [InlineData(HttpStatusCode.Unauthorized)]
    public async Task AC_REC_23_master_serviceが誤りを返すと503の例外にする(HttpStatusCode status)
    {
        var exception = await Assert.ThrowsAsync<ServiceUnavailableException>(() =>
            CreateClient(StubHttpHandler.Returning(status)).ResolveAsync(Query, CancellationToken.None));

        Assert.Equal(MasterDirectoryClient.UnavailableMessage, exception.Message);
    }

    [Fact]
    public async Task AC_REC_23_つながらないと503の例外にする()
    {
        var handler = new StubHttpHandler(_ => throw new HttpRequestException("接続できません"));

        await Assert.ThrowsAsync<ServiceUnavailableException>(() => CreateClient(handler).ResolveAsync(Query, CancellationToken.None));
    }

    [Fact]
    public async Task 応答の形が読めないと503の例外にする()
    {
        await Assert.ThrowsAsync<ServiceUnavailableException>(() =>
            CreateClient(StubHttpHandler.Returning(HttpStatusCode.OK, "<html>")).ResolveAsync(Query, CancellationToken.None));
    }

    [Fact]
    public async Task 呼んだ側が取り消したときは503にせず取り消しのまま伝える()
    {
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();
        var handler = new StubHttpHandler(_ => throw new TaskCanceledException());

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => CreateClient(handler).ResolveAsync(Query, cts.Token));
    }
}
