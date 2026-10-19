namespace Gateway.Security;

/// <summary>
/// 管理用の鍵の設定(マスタの管理 4.3、決定の記録 0009)。値は環境変数 ADMIN_KEY で渡す。
/// </summary>
public sealed class AdminKeyOptions
{
    public const string EnvironmentVariable = "ADMIN_KEY";
    public const int MinLength = 16;

    /// <summary>
    /// 管理用の API の接頭辞。
    /// この下への要求だけに鍵と回数の制限をかける
    /// </summary>
    public const string PathPrefix = "/api/v1/admin";

    public const string HeaderName = "X-Admin-Key";

    /// <summary>
    /// 同じ IP から 1 分に呼べる回数
    /// </summary>
    public const int PermitsPerMinute = 60;

    public string Key { get; }

    private AdminKeyOptions(string key) => Key = key;

    /// <summary>
    /// 環境変数から読む。ないか 16 文字より短いときは、起動を止める。
    /// 例外の文言には変数の名前だけを書き、値は書かない。
    /// </summary>
    /// <param name="value">環境変数 ADMIN_KEY の値。</param>
    /// <returns>鍵を持つ設定。</returns>
    public static AdminKeyOptions FromEnvironment(string? value)
    {
        if (string.IsNullOrEmpty(value) || value.Length < MinLength)
        {
            throw new InvalidOperationException(
                $"{EnvironmentVariable} が設定されていないか、{MinLength} 文字より短いです。.env に {MinLength} 文字以上の値を書いてください");
        }

        return new AdminKeyOptions(value);
    }
}
