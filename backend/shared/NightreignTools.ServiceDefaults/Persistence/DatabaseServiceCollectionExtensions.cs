using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace NightreignTools.ServiceDefaults.Persistence;

/// <summary>サービスのデータベースの接続文字列。環境変数 <c>ConnectionStrings__Default</c> から読む。</summary>
public sealed class DatabaseOptions
{
    public const string ConnectionStringName = "Default";

    public string ConnectionString { get; set; } = "";
}

/// <summary>
/// 各サービスの Infrastructure から呼ぶ、データベースの共通の登録(データベースの共通仕様 5.1、5.5)。
/// </summary>
public static class DatabaseServiceCollectionExtensions
{
    /// <summary>どのサービスのデータベースも、スキーマは game の 1 つとする。</summary>
    public const string Schema = "game";

    /// <summary>マイグレーションの履歴のテーブル。game のスキーマに置く。</summary>
    public const string MigrationsHistoryTable = "__ef_migrations_history";

    /// <summary>
    /// DbContext と、起動のときのマイグレーションを登録する。接続文字列がないときは、起動の時点で止める(サービスの共通構成 5.7)。
    /// </summary>
    public static IServiceCollection AddServiceDatabase<TContext>(this IServiceCollection services)
        where TContext : DbContext
    {
        services.AddOptions<DatabaseOptions>()
            .Configure<IConfiguration>((options, configuration) =>
                options.ConnectionString = configuration.GetConnectionString(DatabaseOptions.ConnectionStringName) ?? "")
            .Validate(options => !string.IsNullOrWhiteSpace(options.ConnectionString),
                $"ConnectionStrings__{DatabaseOptions.ConnectionStringName} が設定されていません。環境変数で渡してください")
            .ValidateOnStart();

        services.AddDbContext<TContext>((provider, options) =>
            UseServiceDatabase(options, provider.GetRequiredService<IOptions<DatabaseOptions>>().Value.ConnectionString));

        services.AddHostedService<DatabaseMigrationService<TContext>>();
        return services;
    }

    /// <summary>
    /// 接続の設定。アプリと、マイグレーションを作る道具(IDesignTimeDbContextFactory)の両方から使う。
    /// 列とテーブルの名前はスネークケースにする(データベースの共通仕様 5.2)。
    /// </summary>
    public static DbContextOptionsBuilder UseServiceDatabase(DbContextOptionsBuilder options, string connectionString) =>
        options
            .UseNpgsql(connectionString, npgsql => npgsql.MigrationsHistoryTable(MigrationsHistoryTable, Schema))
            .UseSnakeCaseNamingConvention();
}
