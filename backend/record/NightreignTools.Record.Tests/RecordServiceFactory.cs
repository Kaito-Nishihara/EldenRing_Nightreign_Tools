using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using NightreignTools.ServiceDefaults;
using NightreignTools.ServiceDefaults.Persistence;
using NightreignTools.ServiceDefaults.Security;

namespace NightreignTools.Record.Tests;

/// <summary>
/// テストのために record-service を起動する。サービス間キーは決まった値にし、テスト用のデータベースにつなぐ。
/// 起動のときに、テスト用のデータベースへマイグレーションが適用される。
/// </summary>
public class RecordServiceFactory : WebApplicationFactory<Program>
{
    public const string ServiceKey = "test-service-key";

    public RecordServiceFactory()
    {
        TestDatabase.EnsureFresh();
    }

    /// <summary>起動に渡すサービス間キー。起動の失敗を確かめるテストで空にする。</summary>
    protected virtual string ConfiguredServiceKey => ServiceKey;

    /// <summary>起動に渡す接続文字列。接続の失敗を確かめるテストで替える。</summary>
    protected virtual string ConfiguredConnectionString => TestDatabase.ConnectionString;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment(ServiceDefaultsExtensions.TestingEnvironment);
        builder.UseSetting(ServiceKeyOptions.ConfigKey, ConfiguredServiceKey);
        builder.UseSetting($"ConnectionStrings:{DatabaseOptions.ConnectionStringName}", ConfiguredConnectionString);
    }
}
