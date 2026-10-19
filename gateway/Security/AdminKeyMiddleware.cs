using System.Security.Cryptography;
using System.Text;
using Gateway.Errors;

namespace Gateway.Security;

/// <summary>
/// 管理用の API(/api/v1/admin の下)への要求で、X-Admin-Key を確かめる(マスタの管理 4.3)。
/// 鍵がないときと合わないときで、応答を変えない。合わなかったときは、そのことと送り元の IP だけをログに出す。
/// </summary>
/// <param name="next">鍵が合ったときに渡す、次のミドルウェア。</param>
/// <param name="options">正しい管理用の鍵。</param>
/// <param name="logger">鍵が合わなかったことと送り元の IP を書くログ。鍵の値は書かない。</param>
public sealed class AdminKeyMiddleware(RequestDelegate next, AdminKeyOptions options, ILogger<AdminKeyMiddleware> logger)
{
    public const string InvalidKeyMessage = "管理用の鍵が違います";

    // 長さの違いで中身を推測されないよう、比べる前に同じ長さのハッシュにそろえる
    private readonly byte[] _expectedHash = SHA256.HashData(Encoding.UTF8.GetBytes(options.Key));

    public async Task InvokeAsync(HttpContext context)
    {
        var provided = context.Request.Headers[AdminKeyOptions.HeaderName].ToString();
        var providedHash = SHA256.HashData(Encoding.UTF8.GetBytes(provided));
        if (CryptographicOperations.FixedTimeEquals(providedHash, _expectedHash))
        {
            await next(context);
            return;
        }

        logger.LogWarning("管理用の鍵が合わない要求を拒みました。送り元 {RemoteIp}", context.Connection.RemoteIpAddress);
        await GatewayErrorResponse.WriteAsync(context, StatusCodes.Status401Unauthorized, InvalidKeyMessage);
    }
}
