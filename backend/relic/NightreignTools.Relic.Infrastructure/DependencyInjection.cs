using Microsoft.Extensions.DependencyInjection;
using NightreignTools.Relic.Application.RelicEffects;
using NightreignTools.Relic.Infrastructure.Persistence;
using NightreignTools.Relic.Infrastructure.Repositories;
using NightreignTools.ServiceDefaults.Persistence;

namespace NightreignTools.Relic.Infrastructure;

/// <summary>
/// Infrastructure の層の登録(サービスの共通構成 5.3)。DbContext とリポジトリの実装をここに登録する。
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // nightreign_relic への接続と、起動のときのマイグレーション(データベースの共通仕様 5.5)
        services.AddServiceDatabase<RelicDbContext>();
        services.AddScoped<IRelicEffectReadRepository, RelicEffectReadRepository>();
        return services;
    }
}
