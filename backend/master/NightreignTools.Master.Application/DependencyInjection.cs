using Microsoft.Extensions.DependencyInjection;
using NightreignTools.Master.Application.Admin;
using NightreignTools.Master.Application.Masters;
using NightreignTools.Master.Application.References;

namespace NightreignTools.Master.Application;

/// <summary>Application の層の登録(サービスの共通構成 5.3)。アプリケーションサービスは、機能を足すたびにここに登録する。</summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IMasterCatalogService, MasterCatalogService>();
        services.AddScoped<IReferenceResolver, ReferenceResolver>();

        // 管理用(マスタの管理 4.2)。種類を足すときは、ここに 1 行足す。TimeProvider は ServiceDefaults が登録する
        services.AddScoped<IMapPlaceAdmin<SaveSpawnPointRequest, AdminSpawnPointDto>, SpawnPointAdmin>();
        services.AddScoped<IMapPlaceAdmin<SaveShrinkZoneRequest, AdminShrinkZoneDto>, ShrinkZoneAdmin>();
        services.AddScoped<IMapPlaceAdmin<SaveSecondShrinkPointRequest, AdminSecondShrinkPointDto>, SecondShrinkPointAdmin>();
        services.AddScoped<IMapPlaceAdmin<SaveBasePointRequest, AdminBasePointDto>, BasePointAdmin>();
        return services;
    }
}
