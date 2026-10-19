using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using NightreignTools.ServiceDefaults.ErrorHandling;

namespace NightreignTools.ServiceDefaults.Security;

/// <summary>
/// Gateway か、ほかのサービスを通った要求だけを通す(API の共通仕様 4.6、4.8)。
/// <c>/health</c> を除くすべてのパスで、ルートの有無にかかわらず確かめる。キーのない要求への 404 と 401 の違いで、どのパスがあるかを知られないためである。
/// </summary>
public sealed class ServiceKeyMiddleware
{
    public const string HeaderName = "X-Service-Key";

    /// <summary>
    /// compose の稼働の確認に使うので、キーなしで答えるパス。
    /// </summary>
    public const string HealthPath = "/health";

    internal const string UnauthorizedMessage = "サービス間キーがないか、誤っています";

    private readonly RequestDelegate _next;

    // 比べる前に両方をハッシュにし、長さの違いからも中身を推測されないようにする
    private readonly byte[] _expectedHash;

    public ServiceKeyMiddleware(RequestDelegate next, IOptions<ServiceKeyOptions> options)
    {
        _next = next;
        _expectedHash = Hash(options.Value.Key);
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Path.Equals(HealthPath, StringComparison.OrdinalIgnoreCase)
            && !IsValid(context.Request.Headers[HeaderName]))
        {
            await ErrorResponseWriter.WriteAsync(context, StatusCodes.Status401Unauthorized, UnauthorizedMessage, context.RequestAborted);
            return;
        }

        await _next(context);
    }

    private bool IsValid(StringValues values)
    {
        if (values.Count != 1 || string.IsNullOrEmpty(values[0]))
        {
            return false;
        }

        return CryptographicOperations.FixedTimeEquals(Hash(values[0]!), _expectedHash);
    }

    private static byte[] Hash(string value) => SHA256.HashData(Encoding.UTF8.GetBytes(value));
}
