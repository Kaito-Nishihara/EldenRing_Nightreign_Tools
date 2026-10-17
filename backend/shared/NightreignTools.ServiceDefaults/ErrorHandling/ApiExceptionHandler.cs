using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NightreignTools.SharedKernel;

namespace NightreignTools.ServiceDefaults.ErrorHandling;

/// <summary>
/// 例外を <see cref="ErrorResponse"/> の応答に変える(サービスの共通構成 5.6)。
/// 状態は例外の型で決め、文言では決めない。
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
            NotFoundException e => (StatusCodes.Status404NotFound, e.Message),
            BusinessRuleException e => (StatusCodes.Status400BadRequest, e.Message),
            ConflictException e => (StatusCodes.Status409Conflict, e.Message),
            ServiceUnavailableException e => (StatusCodes.Status503ServiceUnavailable, e.Message),
            // JSON が読めない、必須のクエリがないなど、エンドポイントに届く前の入力の誤り
            BadHttpRequestException e => (e.StatusCode, BadRequestMessage),
            _ => (StatusCodes.Status500InternalServerError, InternalErrorMessage),
        };

        Log(context, exception, status, message);
        await ErrorResponseWriter.WriteAsync(context, status, message, cancellationToken);
        return true;
    }

    private void Log(HttpContext context, Exception exception, int status, string message)
    {
        var method = context.Request.Method;
        var path = context.Request.Path;
        switch (status)
        {
            case StatusCodes.Status500InternalServerError:
                logger.LogError(exception, "想定していない誤り {Method} {Path}", method, path);
                break;
            case StatusCodes.Status503ServiceUnavailable:
                logger.LogWarning(exception, "{Status} {Method} {Path}: {Message}", status, method, path, message);
                break;
            default:
                logger.LogInformation("{Status} {Method} {Path}: {Message}", status, method, path, message);
                break;
        }
    }
}
