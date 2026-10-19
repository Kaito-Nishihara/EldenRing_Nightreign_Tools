namespace NightreignTools.SharedKernel;

/// <summary>
/// エンティティとアプリケーションサービスが投げる例外の基底(サービスの共通構成 5.6)。
/// HTTP の状態は知らない。ServiceDefaults の例外ハンドラが、例外の型から状態を決める。
/// 文言は、そのまま利用者に見せる。
/// </summary>
/// <param name="message">利用者に見せる文言。</param>
/// <param name="innerException">もとになった例外。なければ null。</param>
public abstract class AppException(string message, Exception? innerException = null) : Exception(message, innerException);

/// <summary>
/// 識別子の行がない。
/// </summary>
/// <param name="message">利用者に見せる文言。どの識別子がないかを書く。</param>
public sealed class NotFoundException(string message) : AppException(message);

/// <summary>
/// 業務の規則に反する。
/// 例: 日と段階の組が重複する。
/// </summary>
/// <param name="message">利用者に見せる文言。どの規則に反したかを書く。</param>
public sealed class BusinessRuleException(string message) : AppException(message);

/// <summary>
/// 今の状態と食い違う操作。
/// </summary>
/// <param name="message">利用者に見せる文言。今の状態の何と食い違うかを書く。</param>
public sealed class ConflictException(string message) : AppException(message);

/// <summary>
/// 呼んだ別のサービスが応答しない。
/// </summary>
/// <param name="message">利用者に見せる文言。どのサービスが応答しないかを書く。</param>
/// <param name="innerException">呼び出しで起きた例外。なければ null。</param>
public sealed class ServiceUnavailableException(string message, Exception? innerException = null)
    : AppException(message, innerException);
