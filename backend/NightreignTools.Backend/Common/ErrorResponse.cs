namespace NightreignTools.Backend.Common;

/// <summary>
/// すべてのエラーの応答の形(API の共通仕様 4.3)。
/// </summary>
/// <param name="Status">HTTP の状態の数</param>
/// <param name="Error">HTTP の状態の英語の名前。例: Not Found</param>
/// <param name="Message">利用者に見せてよい日本語の説明</param>
/// <param name="Timestamp">エラーを返した日時(UTC)</param>
public sealed record ErrorResponse(int Status, string Error, string Message, DateTime Timestamp);
