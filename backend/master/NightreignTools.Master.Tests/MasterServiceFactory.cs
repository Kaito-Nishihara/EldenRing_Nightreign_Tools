using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using NightreignTools.ServiceDefaults;
using NightreignTools.ServiceDefaults.Security;

namespace NightreignTools.Master.Tests;

/// <summary>
/// テストのために master-service を起動する。サービス間キーは決まった値にする。
/// </summary>
public class MasterServiceFactory : WebApplicationFactory<Program>
{
    public const string ServiceKey = "test-service-key";

    /// <summary>起動に渡すサービス間キー。起動の失敗を確かめるテストで空にする。</summary>
    protected virtual string ConfiguredServiceKey => ServiceKey;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment(ServiceDefaultsExtensions.TestingEnvironment);
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
