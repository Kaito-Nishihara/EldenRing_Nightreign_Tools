using System.Net;
using System.Text.Json;
using NightreignTools.Backend.Common;

namespace NightreignTools.Backend.Tests.Common;

/// <summary>
/// サービス間キーの検証(API の共通仕様 4.6)。
/// </summary>
public class ServiceKeyTests(BackendFactory factory) : IClassFixture<BackendFactory>
{
    [Fact]
    public async Task AC_API_03_キーなしで呼ぶと401が返る()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/v1/bosses");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        var body = await ReadJsonAsync(response);
        Assert.Equal(401, body.GetProperty("status").GetInt32());
        Assert.Equal("Unauthorized", body.GetProperty("error").GetString());
    }

    [Fact]
    public async Task AC_API_04_誤ったキーで呼ぶと401が返る()
    {
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add(ServiceKeyMiddleware.HeaderName, "wrong");

        var response = await client.GetAsync("/api/v1/bosses");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task AC_API_09_healthはキーなしで200を返す()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData("/api/v1/no-such-path")]
    [InlineData("/no-such-path")]
    [InlineData("/openapi/v1.json")]
    public async Task AC_SVC_07_キーなしの要求はルートの有無にかかわらず401を返す(string path)
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync(path);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task 正しいキーで存在しないパスを呼ぶと404がエラーの形で返る()
    {
        var client = factory.CreateClientWithServiceKey();

        var response = await client.GetAsync("/api/v1/no-such-path");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var body = await ReadJsonAsync(response);
        Assert.Equal("パス '/api/v1/no-such-path' は存在しません", body.GetProperty("message").GetString());
        Assert.EndsWith("Z", body.GetProperty("timestamp").GetString());
    }

    private static async Task<JsonElement> ReadJsonAsync(HttpResponseMessage response)
    {
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return document.RootElement.Clone();
    }
}
