using Microsoft.Extensions.DependencyInjection;
using NightreignTools.Record.Infrastructure.Persistence;
using NightreignTools.ServiceDefaults.Persistence;

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
        return services;
    }
}
