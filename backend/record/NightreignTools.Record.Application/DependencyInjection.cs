using Microsoft.Extensions.DependencyInjection;
using NightreignTools.Record.Application.Patterns;
using NightreignTools.Record.Application.PlayRecords;

namespace NightreignTools.Record.Application;

/// <summary>Application の層の登録(サービスの共通構成 5.3)。アプリケーションサービスは、機能を足すたびにここに登録する。</summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IPlayRecordService, PlayRecordService>();
        services.AddScoped<IPatternSearchService, PatternSearchService>();
        return services;
    }
}
