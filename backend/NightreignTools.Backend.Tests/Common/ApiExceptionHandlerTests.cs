using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using NightreignTools.Backend.Common;

namespace NightreignTools.Backend.Tests.Common;

/// <summary>
/// 例外から応答への変換(サービスの共通構成 5.4、API の共通仕様 4.3)。
/// </summary>
public class ApiExceptionHandlerTests
{
    private static readonly DateTimeOffset Now = new(2026, 10, 17, 12, 34, 56, TimeSpan.Zero);

    [Fact]
    public async Task AC_SVC_03_NotFoundExceptionは404になる()
    {
        var (status, body) = await HandleAsync(new NotFoundException("bossId 'unknown' は存在しません"));

        Assert.Equal(404, status);
        Assert.Equal("Not Found", body.GetProperty("error").GetString());
        Assert.Equal("bossId 'unknown' は存在しません", body.GetProperty("message").GetString());
    }

    [Fact]
    public async Task AC_SVC_04_BusinessRuleExceptionは400になる()
    {
        var (status, body) = await HandleAsync(new BusinessRuleException("day=1, phase=1 が重複しています"));

        Assert.Equal(400, status);
        Assert.Equal("day=1, phase=1 が重複しています", body.GetProperty("message").GetString());
    }

    [Fact]
    public async Task AC_SVC_05_ConflictExceptionは409になる()
    {
        var (status, _) = await HandleAsync(new ConflictException("この収縮候補地は 12 件の記録から参照されているため、削除できません"));

        Assert.Equal(409, status);
    }

    [Fact]
    public async Task AC_API_01_エラーの応答はstatusとerrorとmessageとtimestampの4つの項目を持つ()
    {
        var (_, body) = await HandleAsync(new NotFoundException("x"));

        var names = body.EnumerateObject().Select(p => p.Name).Order().ToArray();
        Assert.Equal(["error", "message", "status", "timestamp"], names);
        Assert.Equal(404, body.GetProperty("status").GetInt32());
    }

    [Fact]
    public async Task AC_API_08_timestampは末尾にZの付いたUTCの日時である()
    {
        var (_, body) = await HandleAsync(new NotFoundException("x"));

        var timestamp = body.GetProperty("timestamp").GetString();
        Assert.Equal("2026-10-17T12:34:56Z", timestamp);
    }

    [Fact]
    public async Task AC_API_06_想定していない例外は500になりmessageにSQLとスタックトレースが入らない()
    {
        Exception thrown;
        try
        {
            throw new InvalidOperationException("SELECT * FROM game.play_records failed");
        }
        catch (InvalidOperationException ex)
        {
            thrown = ex;
        }

        var (status, body) = await HandleAsync(thrown);

        Assert.Equal(500, status);
        var message = body.GetProperty("message").GetString();
        Assert.Equal("内部サーバーエラーが発生しました", message);
        Assert.DoesNotContain("SELECT", body.GetRawText());
        Assert.DoesNotContain(nameof(AC_API_06_想定していない例外は500になりmessageにSQLとスタックトレースが入らない), body.GetRawText());
    }

    /// <summary>ハンドラに例外を渡し、書かれた状態と本文を返す。</summary>
    private static async Task<(int Status, JsonElement Body)> HandleAsync(Exception exception)
    {
        var services = new ServiceCollection()
            .AddSingleton<TimeProvider>(new FixedTimeProvider(Now))
            .BuildServiceProvider();
        var context = new DefaultHttpContext { RequestServices = services };
        context.Response.Body = new MemoryStream();

        var handler = new ApiExceptionHandler(NullLogger<ApiExceptionHandler>.Instance);
        var handled = await handler.TryHandleAsync(context, exception, CancellationToken.None);
        Assert.True(handled);

        context.Response.Body.Position = 0;
        using var document = await JsonDocument.ParseAsync(context.Response.Body);
        return (context.Response.StatusCode, document.RootElement.Clone());
    }

    private sealed class FixedTimeProvider(DateTimeOffset now) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => now;
    }
}
