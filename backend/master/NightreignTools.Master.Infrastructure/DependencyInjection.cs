using Microsoft.Extensions.DependencyInjection;
using NightreignTools.Master.Application.Masters;
using NightreignTools.Master.Infrastructure.Persistence;
using NightreignTools.Master.Infrastructure.Repositories;
using NightreignTools.ServiceDefaults.Persistence;

namespace NightreignTools.Master.Infrastructure;

/// <summary>Infrastructure の層の登録(サービスの共通構成 5.3)。DbContext とリポジトリの実装をここに登録する。</summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // nightreign_master への接続と、起動のときのマイグレーション(データベースの共通仕様 5.5)
        services.AddServiceDatabase<MasterDbContext>();

        services.AddScoped<IMasterReadRepository, MasterReadRepository>();
        return services;
    }
}
