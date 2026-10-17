namespace NightreignTools.ServiceDefaults.Security;

/// <summary>
/// Gateway と各サービスの間、サービスどうしの間のサービス間キー。環境変数 <c>SERVICE_KEY</c> から読む。
/// </summary>
public sealed class ServiceKeyOptions
{
    public const string ConfigKey = "SERVICE_KEY";

    public string Key { get; set; } = "";
}
