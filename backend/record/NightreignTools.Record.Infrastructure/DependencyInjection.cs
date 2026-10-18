using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NightreignTools.Record.Application.Masters;
using NightreignTools.Record.Application.Patterns;
using NightreignTools.Record.Application.PlayRecords;
using NightreignTools.Record.Infrastructure.Masters;
using NightreignTools.Record.Infrastructure.Persistence;
using NightreignTools.Record.Infrastructure.Repositories;
using NightreignTools.ServiceDefaults.Persistence;
using NightreignTools.ServiceDefaults.Security;

namespace NightreignTools.Record.Infrastructure;

/// <summary>
/// Infrastructure の層の登録(サービスの共通構成 5.3)。DbContext とリポジトリの実装、master-service のクライアントをここに登録する。
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // nightreign_record への接続と、起動のときのマイグレーション(データベースの共通仕様 5.5)
        services.AddServiceDatabase<RecordDbContext>();
        services.AddScoped<IPlayRecordRepository, PlayRecordRepository>();
        services.AddScoped<IPatternRecordReader, PatternRecordReader>();

        services.AddMasterDirectory();
        return services;
    }

    /// <summary>
    /// master-service のクライアント(サービスの共通構成 5.5)。呼び先がないときは、起動の時点で止める(AC-SVC-10)。
    /// </summary>
    private static void AddMasterDirectory(this IServiceCollection services)
    {
        services.AddOptions<MasterServiceOptions>()
            .Configure<IConfiguration>((options, configuration) => options.Url = configuration[MasterServiceOptions.ConfigKey] ?? "")
            .Validate(options => options.IsValid(),
                $"{MasterServiceOptions.ConfigKey} が設定されていないか、http か https の URL ではありません。環境変数で渡してください")
            .ValidateOnStart();

        services.AddHttpClient<IMasterDirectory, MasterDirectoryClient>((provider, client) =>
            {
                var url = provider.GetRequiredService<IOptions<MasterServiceOptions>>().Value.Url;
                client.BaseAddress = new Uri(url.EndsWith('/') ? url : url + "/");
                // Gateway と同じサービス間キーを付ける
                client.DefaultRequestHeaders.Add(ServiceKeyMiddleware.HeaderName, provider.GetRequiredService<IOptions<ServiceKeyOptions>>().Value.Key);
            })
            .AddStandardResilienceHandler(options =>
            {
                // 1 回の試行は 3 秒、全体は 10 秒。通信の失敗と 5xx のときに 2 回まで再試行する
                options.AttemptTimeout.Timeout = MasterServiceOptions.AttemptTimeout;
                options.TotalRequestTimeout.Timeout = MasterServiceOptions.TotalTimeout;
                options.Retry.MaxRetryAttempts = MasterServiceOptions.MaxRetryAttempts;
            });
    }
}
