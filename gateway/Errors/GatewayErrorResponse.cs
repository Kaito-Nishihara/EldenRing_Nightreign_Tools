using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.WebUtilities;

namespace Gateway.Errors;

/// <summary>
/// Gateway が自分で返すエラーの本文。各サービスと同じ { status, error, message, timestamp } の形にする(API の共通仕様 4.3)。
/// </summary>
public static class GatewayErrorResponse
{
    /// <summary>
    /// 振り分け先のないパス(404)と、サービスにつなげなかった応答(502、504)に本文を付ける。
    /// サービスが本文付きで返した応答は、そのまま通す。
    /// </summary>
    public static Task WriteStatusCodePageAsync(StatusCodeContext context)
    {
        var http = context.HttpContext;
        var status = http.Response.StatusCode;
        var message = status switch
        {
            StatusCodes.Status404NotFound => $"パス '{http.Request.Path}' は存在しません",
            StatusCodes.Status502BadGateway or StatusCodes.Status504GatewayTimeout => "サーバーに接続できません。時間をおいてやり直してください",
            _ => ReasonPhrases.GetReasonPhrase(status),
        };

        return http.Response.WriteAsJsonAsync(new
        {
            status,
            error = ReasonPhrases.GetReasonPhrase(status),
            message,
            timestamp = DateTime.UtcNow,
        }, http.RequestAborted);
    }
}
