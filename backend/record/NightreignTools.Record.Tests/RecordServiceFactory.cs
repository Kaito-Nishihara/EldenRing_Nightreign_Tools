using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using NightreignTools.Record.Infrastructure.Masters;
using NightreignTools.ServiceDefaults;
using NightreignTools.ServiceDefaults.Persistence;
using NightreignTools.ServiceDefaults.Security;

namespace NightreignTools.Record.Tests;

/// <summary>
/// テストのために record-service を起動する。サービス間キーは決まった値にし、テスト用のデータベースにつなぐ。
/// 起動のときに、テスト用のデータベースへマイグレーションが適用される。
/// master-service は起動しない(サービスの共通構成 5.9)。呼び先には、つながらない仮の URL を渡す。
/// </summary>
public class RecordServiceFactory : WebApplicationFactory<Program>
{
    public const string ServiceKey = "test-service-key";
    public const string MasterServiceUrl = "http://master-service.test";

    public RecordServiceFactory()
    {
        TestDatabase.EnsureFresh();
    }

    /// <summary>
    /// 起動に渡すサービス間キー。
    /// 起動の失敗を確かめるテストで空にする。
    /// </summary>
    protected virtual string ConfiguredServiceKey => ServiceKey;

    /// <summary>
    /// 起動に渡す接続文字列。
    /// 接続の失敗を確かめるテストで替える。
    /// </summary>
    protected virtual string ConfiguredConnectionString => TestDatabase.ConnectionString;

    /// <summary>
    /// 起動に渡す master-service の呼び先。
    /// 起動の失敗を確かめるテストで空にする。
    /// </summary>
    protected virtual string ConfiguredMasterServiceUrl => MasterServiceUrl;

    /// <summary>
    /// 登録を差し替える。
    /// master-service のクライアントを偽物にするテストで使う。
    /// </summary>
    /// <param name="services">差し替える登録。</param>
    protected virtual void ConfigureTestServices(IServiceCollection services)
    {
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment(ServiceDefaultsExtensions.TestingEnvironment);
        builder.UseSetting(ServiceKeyOptions.ConfigKey, ConfiguredServiceKey);
        builder.UseSetting($"ConnectionStrings:{DatabaseOptions.ConnectionStringName}", ConfiguredConnectionString);
        builder.UseSetting(MasterServiceOptions.ConfigKey, ConfiguredMasterServiceUrl);
        builder.ConfigureTestServices(ConfigureTestServices);
    }
}
