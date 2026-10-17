using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NightreignTools.ServiceDefaults.ErrorHandling;
using NightreignTools.ServiceDefaults.Security;

namespace NightreignTools.ServiceDefaults;

/// <summary>
/// 3 つのサービスの Program.cs から呼ぶ、共通の登録と組み立て(サービスの共通構成 5.4)。
/// </summary>
public static class ServiceDefaultsExtensions
{
    /// <summary>テストの WebApplicationFactory が使う環境の名前。</summary>
    public const string TestingEnvironment = "Testing";

    /// <summary>エラーの形、例外の変換、サービス間キー、/health、OpenAPI を登録する。</summary>
    public static WebApplicationBuilder AddServiceDefaults(this WebApplicationBuilder builder)
    {
        var services = builder.Services;
        services.AddSingleton(TimeProvider.System);

        // サービス間キーがないときは、起動の時点で止める(サービスの共通構成 5.7)
        services.AddOptions<ServiceKeyOptions>()
            .Configure<IConfiguration>((options, configuration) => options.Key = configuration[ServiceKeyOptions.ConfigKey] ?? "")
            .Validate(options => !string.IsNullOrWhiteSpace(options.Key),
                $"{ServiceKeyOptions.ConfigKey} が設定されていません。環境変数で渡してください")
            .ValidateOnStart();

        // 例外は ApiExceptionHandler が ErrorResponse の形に変える。
        // 入力の検証の誤りなど、ASP.NET Core が ProblemDetails で書くものも、同じ形に書き替える(既定の書き手より先に登録する)
        services.AddSingleton<IProblemDetailsWriter, ErrorResponseProblemDetailsWriter>();
        services.AddProblemDetails();
        services.AddExceptionHandler<ApiExceptionHandler>();

        services.AddHealthChecks();
        services.AddOpenApi();
        return builder;
    }

    /// <summary>ミドルウェアと、/health、OpenAPI のエンドポイントを組み立てる。機能のエンドポイントは、この後に足す。</summary>
    public static WebApplication UseServiceDefaults(this WebApplication app)
    {
        app.UseExceptionHandler();
        // 本文のないまま終わった 4xx(ルートがないなど)にも、ErrorResponse の本文を付ける
        app.UseStatusCodePages(ErrorResponseWriter.WriteStatusCodePageAsync);
        app.UseMiddleware<ServiceKeyMiddleware>();

        app.MapHealthChecks(ServiceKeyMiddleware.HealthPath);

        // OpenAPI の文書は開発の環境でだけ出す(API の共通仕様 4.7)
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        return app;
    }

    /// <summary>
    /// サービスを動かす。設定の不足、データベースにつなげない、マイグレーションの失敗で起動できなかったときは、
    /// 理由をログに 1 回だけ出し、終了コード 1 を返す(サービスの共通構成 5.7)。
    /// </summary>
    public static async Task<int> RunServiceAsync(this WebApplication app)
    {
        try
        {
            await app.RunAsync();
            return 0;
        }
        // HostAbortedException はテストの WebApplicationFactory が起動を止めるときに使うので、捕まえない。
        // テストの環境では、起動の失敗の理由を WebApplicationFactory へそのまま渡し、テストで確かめられるようにする
        catch (Exception ex) when (ex is not HostAbortedException && !app.Environment.IsEnvironment(TestingEnvironment))
        {
            app.Logger.LogCritical(ex, "起動できませんでした: {Message}", ex.Message);
            return 1;
        }
    }
}
