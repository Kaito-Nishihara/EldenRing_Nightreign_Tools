using Microsoft.AspNetCore.Diagnostics;

namespace NightreignTools.Backend.Common;

/// <summary>
/// 例外を <see cref="ErrorResponse"/> の応答に変える(サービスの共通構成 5.4)。
/// 想定していない例外は 500 にし、応答には固定の文言だけを返す。スタックトレースや SQL を応答に出さないためである。
/// </summary>
public sealed class ApiExceptionHandler(ILogger<ApiExceptionHandler> logger) : IExceptionHandler
{
    internal const string InternalErrorMessage = "内部サーバーエラーが発生しました";
    internal const string BadRequestMessage = "要求の形が誤っています";

    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        var (status, message) = exception switch
        {
            ApiException api => (api.StatusCode, api.Message),
            // JSON が読めない、必須のクエリがないなど、エンドポイントに届く前の入力の誤り
            BadHttpRequestException bad => (bad.StatusCode, BadRequestMessage),
            _ => (StatusCodes.Status500InternalServerError, InternalErrorMessage),
        };

        if (status >= StatusCodes.Status500InternalServerError)
        {
            logger.LogError(exception, "想定していない誤り {Method} {Path}", context.Request.Method, context.Request.Path);
        }
        else
        {
            logger.LogInformation("{Status} {Method} {Path}: {Message}", status, context.Request.Method, context.Request.Path, message);
        }

        await ErrorResponseWriter.WriteAsync(context, status, message, cancellationToken);
        return true;
    }
}
