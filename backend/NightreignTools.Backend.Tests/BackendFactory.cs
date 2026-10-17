using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using NightreignTools.Backend.Common;

namespace NightreignTools.Backend.Tests;

/// <summary>
/// テストのためにバックエンドを起動する。サービス間キーは決まった値にする。
/// </summary>
public class BackendFactory : WebApplicationFactory<Program>
{
    public const string ServiceKey = "test-service-key";

    /// <summary>起動に渡すサービス間キー。起動の失敗を確かめるテストで空にする。</summary>
    protected virtual string ConfiguredServiceKey => ServiceKey;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.UseSetting(ServiceKeyOptions.ConfigKey, ConfiguredServiceKey);
    }

    /// <summary>Gateway と同じく、正しいサービス間キーを付けて呼ぶクライアント。</summary>
    public HttpClient CreateClientWithServiceKey()
    {
        var client = CreateClient();
        client.DefaultRequestHeaders.Add(ServiceKeyMiddleware.HeaderName, ServiceKey);
        return client;
    }
}
