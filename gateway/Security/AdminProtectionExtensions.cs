using System.Threading.RateLimiting;
using Gateway.Errors;

namespace Gateway.Security;

/// <summary>
/// 管理用の API の守り(マスタの管理 4.3、決定の記録 0009)。登録と、要求の流れへの組み込みをまとめる。
/// </summary>
public static class AdminProtectionExtensions
{
    public const string TooManyRequestsMessage = "しばらく時間をおいてやり直してください";

    /// <summary>
    /// 管理用の鍵を読み、回数の制限を登録する。鍵がないか短いときは、ここで起動を止める。
    /// </summary>
    /// <param name="services">登録先。</param>
    /// <param name="adminKey">環境変数 ADMIN_KEY の値。ないか 16 文字より短いと起動を止める。</param>
    /// <returns>続けて登録できるように、受け取った services を返す。</returns>
    public static IServiceCollection AddAdminProtection(this IServiceCollection services, string? adminKey)
    {
        services.AddSingleton(AdminKeyOptions.FromEnvironment(adminKey));

        services.AddRateLimiter(limiter =>
        {
            // 送り元の IP ごとに、1 分に 60 回まで。鍵が合っている要求も数える
            limiter.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                RateLimitPartition.GetFixedWindowLimiter(
                    context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = AdminKeyOptions.PermitsPerMinute,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0,
                    }));
            limiter.OnRejected = (rejected, _) =>
                new ValueTask(GatewayErrorResponse.WriteAsync(rejected.HttpContext, StatusCodes.Status429TooManyRequests, TooManyRequestsMessage));
        });

        return services;
    }

    /// <summary>
    /// /api/v1/admin の下への要求にだけ、回数の制限と鍵の確かめをかける。プレイヤー向けの API には何もしない。
    /// 回数の制限を先にかけ、鍵を当てようとする要求も止める。
    /// </summary>
    /// <param name="app">組み立て中の Gateway。</param>
    /// <returns>続けて組み立てられるように、受け取った app を返す。</returns>
    public static IApplicationBuilder UseAdminProtection(this IApplicationBuilder app) =>
        app.UseWhen(
            context => context.Request.Path.StartsWithSegments(AdminKeyOptions.PathPrefix, StringComparison.OrdinalIgnoreCase),
            admin =>
            {
                admin.UseRateLimiter();
                admin.UseMiddleware<AdminKeyMiddleware>();
            });
}
