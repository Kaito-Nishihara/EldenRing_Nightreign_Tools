using Microsoft.Extensions.DependencyInjection;

namespace NightreignTools.Master.Infrastructure;

/// <summary>Infrastructure の層の登録(サービスの共通構成 5.3)。DbContext とリポジトリの実装をここに登録する。</summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        return services;
    }
}
