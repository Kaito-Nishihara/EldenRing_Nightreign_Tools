using Gateway.Errors;
using Gateway.Security;
using Yarp.ReverseProxy.Transforms;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// サービス登録
// ==========================================

// CORS 設定。許すオリジンは環境変数 CORS_ORIGINS(カンマ区切り)で渡す。省いたら http://localhost:3000(API の共通仕様 4.6)
var corsOrigins = (Environment.GetEnvironmentVariable("CORS_ORIGINS") ?? "")
    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
if (corsOrigins.Length == 0)
{
    corsOrigins = ["http://localhost:3000"];
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy
            .WithOrigins(corsOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// ヘルスチェック
builder.Services.AddHealthChecks();

// 振り分け先のサービスの URL を環境変数で上書きする(サービスの共通構成 5.7)。
// record-service は master-service を呼ぶので(最長 10 秒)、record-cluster の期限は 15 秒にしてある
OverrideDestination("MASTER_SERVICE_URL", "master-cluster", "master");
OverrideDestination("RECORD_SERVICE_URL", "record-cluster", "record");

void OverrideDestination(string variable, string cluster, string destination)
{
    var url = Environment.GetEnvironmentVariable(variable);
    if (!string.IsNullOrEmpty(url))
    {
        builder.Configuration[$"ReverseProxy:Clusters:{cluster}:Destinations:{destination}:Address"] = url;
    }
}

// サービス間認証キーを取得
var serviceKey = Environment.GetEnvironmentVariable("SERVICE_KEY")
    ?? builder.Configuration["ServiceKey"]
    ?? throw new InvalidOperationException("SERVICE_KEY が設定されていません。環境変数または appsettings.json で設定してください。");

// 管理用の鍵と、管理用の API の回数の制限(マスタの管理 4.3)。ADMIN_KEY がないか 16 文字より短いときは、ここで起動を止める
builder.Services.AddAdminProtection(Environment.GetEnvironmentVariable(AdminKeyOptions.EnvironmentVariable));

// YARP リバースプロキシ設定(appsettings.json から読み込み)。振り分けの表は API の共通仕様 4.6
builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddTransforms(transformContext =>
    {
        // 画面から来た X-Service-Key を消し、Gateway の値を付け直す。
        // 確かめに通った X-Admin-Key は、サービスへ渡さない(マスタの管理 4.3)
        transformContext.AddRequestTransform(context =>
        {
            context.ProxyRequest.Headers.Remove("X-Service-Key");
            context.ProxyRequest.Headers.Add("X-Service-Key", serviceKey);
            context.ProxyRequest.Headers.Remove(AdminKeyOptions.HeaderName);
            return ValueTask.CompletedTask;
        });
    });

var app = builder.Build();

// ==========================================
// ミドルウェアパイプライン
// ==========================================

// CORS を最初に適用
app.UseCors("FrontendPolicy");

// Gateway が自分で返す、本文のない 404、502、504 にも、エラーの形の本文を付ける(API の共通仕様 4.3)
app.UseStatusCodePages(GatewayErrorResponse.WriteStatusCodePageAsync);

// ==========================================
// エンドポイント定義(Minimal API)
// ==========================================

// ヘルスチェックエンドポイント
app.MapHealthChecks("/health");

// /api/v1/admin の下だけに、回数の制限と管理用の鍵の確かめをかける。CORS の事前の確かめ(OPTIONS)は UseCors が先に返す
app.UseAdminProtection();

// YARP リバースプロキシをマッピング。/internal/v1 は振り分けないので、Gateway が 404 を返す
app.MapReverseProxy();

app.Run();
