namespace NightreignTools.SharedKernel;

/// <summary>
/// エンティティとアプリケーションサービスが投げる例外の基底(サービスの共通構成 5.6)。
/// HTTP の状態は知らない。ServiceDefaults の例外ハンドラが、例外の型から状態を決める。
/// 文言は、そのまま利用者に見せる。
/// </summary>
public abstract class AppException(string message, Exception? innerException = null) : Exception(message, innerException);

/// <summary>識別子の行がない。</summary>
public sealed class NotFoundException(string message) : AppException(message);

/// <summary>業務の規則に反する。例: 日と段階の組が重複する。</summary>
public sealed class BusinessRuleException(string message) : AppException(message);

/// <summary>今の状態と食い違う操作。</summary>
public sealed class ConflictException(string message) : AppException(message);

/// <summary>呼んだ別のサービスが応答しない。</summary>
public sealed class ServiceUnavailableException(string message, Exception? innerException = null)
    : AppException(message, innerException);
