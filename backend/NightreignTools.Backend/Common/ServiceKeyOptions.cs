namespace NightreignTools.Backend.Common;

/// <summary>
/// Gateway との間のサービス間キー。環境変数 <c>SERVICE_KEY</c> から読む。
/// </summary>
public sealed class ServiceKeyOptions
{
    public const string ConfigKey = "SERVICE_KEY";

    public string Key { get; set; } = "";
}

public static class ServiceKeyServiceCollectionExtensions
{
    /// <summary>
    /// サービス間キーを登録する。値がないときは、起動の時点で止める(サービスの共通構成 5.5)。
    /// </summary>
    public static IServiceCollection AddServiceKey(this IServiceCollection services)
    {
        services.AddOptions<ServiceKeyOptions>()
            .Configure<IConfiguration>((options, configuration) => options.Key = configuration[ServiceKeyOptions.ConfigKey] ?? "")
            .Validate(options => !string.IsNullOrWhiteSpace(options.Key),
                $"{ServiceKeyOptions.ConfigKey} が設定されていません。環境変数で渡してください")
            .ValidateOnStart();
        return services;
    }
}
