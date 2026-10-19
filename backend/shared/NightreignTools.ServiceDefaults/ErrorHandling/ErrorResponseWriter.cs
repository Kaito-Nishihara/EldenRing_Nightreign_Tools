using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;

namespace NightreignTools.ServiceDefaults.ErrorHandling;

/// <summary>
/// エラーの応答を <see cref="ErrorResponse"/> の形で書く。
/// 例外の変換、サービス間キーの検証、本文のない 4xx の応答のどこから書いても、同じ形になるようにここに集める。
/// </summary>
public static class ErrorResponseWriter
{
    public static Task WriteAsync(HttpContext context, int status, string message, CancellationToken cancellationToken)
    {
        var timeProvider = context.RequestServices.GetRequiredService<TimeProvider>();
        var body = new ErrorResponse(
            status,
            ReasonPhrases.GetReasonPhrase(status),
            message,
            timeProvider.GetUtcNow().UtcDateTime);

        context.Response.StatusCode = status;
        return context.Response.WriteAsJsonAsync(body, cancellationToken);
    }

    /// <summary>
    /// ルートがない、メソッドが違うなど、本文を持たないまま終わった応答に本文を付ける(UseStatusCodePages から呼ぶ)。
    /// </summary>
    /// <param name="context">本文を持たないまま終わった応答の文脈。状態は応答に入っているものを使う。</param>
    /// <returns>本文を書き終えると終わるタスク。</returns>
    public static Task WriteStatusCodePageAsync(StatusCodeContext context)
    {
        var http = context.HttpContext;
        var status = http.Response.StatusCode;
        return WriteAsync(http, status, DefaultMessage(status, http.Request.Path), http.RequestAborted);
    }

    private static string DefaultMessage(int status, PathString path) => status switch
    {
        StatusCodes.Status400BadRequest => "要求の形が誤っています",
        StatusCodes.Status404NotFound => $"パス '{path}' は存在しません",
        StatusCodes.Status405MethodNotAllowed => "このパスでは、このメソッドを使えません",
        _ => ReasonPhrases.GetReasonPhrase(status),
    };
}
