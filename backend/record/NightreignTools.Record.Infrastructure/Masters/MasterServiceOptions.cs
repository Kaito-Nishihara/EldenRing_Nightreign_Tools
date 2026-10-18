namespace NightreignTools.Record.Infrastructure.Masters;

/// <summary>
/// master-service の呼び先(サービスの共通構成 5.5、5.7)。環境変数 <c>MASTER_SERVICE_URL</c> から読む。
/// </summary>
public sealed class MasterServiceOptions
{
    public const string ConfigKey = "MASTER_SERVICE_URL";

    /// <summary>1 回の試行の期限。</summary>
    public static readonly TimeSpan AttemptTimeout = TimeSpan.FromSeconds(3);

    /// <summary>再試行を含めた全体の期限。</summary>
    public static readonly TimeSpan TotalTimeout = TimeSpan.FromSeconds(10);

    /// <summary>通信の失敗と 5xx のときの再試行の回数。</summary>
    public const int MaxRetryAttempts = 2;

    public string Url { get; set; } = "";

    /// <summary>http か https の絶対の URL か。</summary>
    public bool IsValid() =>
        Uri.TryCreate(Url, UriKind.Absolute, out var uri) && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
}
