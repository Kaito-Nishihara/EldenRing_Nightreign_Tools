using Microsoft.Extensions.DependencyInjection;

namespace NightreignTools.Record.Application;

/// <summary>
/// Application の層の登録(サービスの共通構成 5.3)。アプリケーションサービスは、機能を足すたびにここに登録する。
/// 記録の保存とパターン検索は、それぞれの機能の PR で足す。
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services;
    }
}
