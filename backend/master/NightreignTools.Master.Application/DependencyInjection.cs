using Microsoft.Extensions.DependencyInjection;
using NightreignTools.Master.Application.Masters;

namespace NightreignTools.Master.Application;

/// <summary>Application の層の登録(サービスの共通構成 5.3)。アプリケーションサービスは、機能を足すたびにここに登録する。</summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IMasterCatalogService, MasterCatalogService>();
        return services;
    }
}
