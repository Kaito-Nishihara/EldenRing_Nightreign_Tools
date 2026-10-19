using Microsoft.Extensions.DependencyInjection;
using NightreignTools.Relic.Application.RelicEffects;

namespace NightreignTools.Relic.Application;

/// <summary>Application の層の登録(サービスの共通構成 5.3)。アプリケーションサービスは、機能を足すたびにここに登録する。</summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IRelicEffectCatalogService, RelicEffectCatalogService>();
        return services;
    }
}
