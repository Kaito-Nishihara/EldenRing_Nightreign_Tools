namespace NightreignTools.Backend.Common;

/// <summary>
/// サービスが投げ、共通の例外ハンドラが HTTP の応答に変える例外の基底(サービスの共通構成 5.4)。
/// 状態は例外の型で決め、文言では決めない。文言はそのまま利用者に見せる。
/// </summary>
public abstract class ApiException(string message) : Exception(message)
{
    public abstract int StatusCode { get; }
}

/// <summary>識別子の行がない(404)。</summary>
public sealed class NotFoundException(string message) : ApiException(message)
{
    public override int StatusCode => StatusCodes.Status404NotFound;
}

/// <summary>業務の規則に反する(400)。例: 日と段階の組が重複する。</summary>
public sealed class BusinessRuleException(string message) : ApiException(message)
{
    public override int StatusCode => StatusCodes.Status400BadRequest;
}

/// <summary>記録から参照されているマスタを消そうとした(409)。</summary>
public sealed class ConflictException(string message) : ApiException(message)
{
    public override int StatusCode => StatusCodes.Status409Conflict;
}
