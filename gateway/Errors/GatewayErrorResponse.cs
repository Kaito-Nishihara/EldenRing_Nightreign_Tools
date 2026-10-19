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
    /// <param name="context">本文を持たないまま終わった応答の文脈。</param>
    /// <returns>本文を書き終えると終わるタスク。</returns>
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

        return WriteBodyAsync(http, status, message);
    }

    /// <summary>
    /// Gateway が自分で応答を返すときに、状態と本文を書く。管理用の鍵の誤り(401)と回数の超過(429)に使う
    /// </summary>
    /// <param name="http">応答を書く要求。</param>
    /// <param name="status">HTTP の状態。</param>
    /// <param name="message">利用者に見せる文言。鍵の値を入れない。</param>
    /// <returns>本文を書き終えると終わるタスク。</returns>
    public static Task WriteAsync(HttpContext http, int status, string message)
    {
        http.Response.StatusCode = status;
        return WriteBodyAsync(http, status, message);
    }

    private static Task WriteBodyAsync(HttpContext http, int status, string message) =>
        http.Response.WriteAsJsonAsync(new
        {
            status,
            error = ReasonPhrases.GetReasonPhrase(status),
            message,
            timestamp = DateTime.UtcNow,
        }, http.RequestAborted);
}
